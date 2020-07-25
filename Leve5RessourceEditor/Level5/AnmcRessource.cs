using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kanvas;
using Komponent.IO;
using Komponent.IO.Streams;
using Kontract;
using Kontract.Interfaces.FileSystem;
using Kontract.Interfaces.Managers;
using Kontract.Interfaces.Plugins.State;
using Kontract.Models.IO;
using Kore.Factories;
using Kore.Managers.Plugins;
using Leve5RessourceEditor.Level5.Models;
using MoreLinq;
using plugin_level5.Compression;

namespace Leve5RessourceEditor.Level5
{
    class AnmcRessource : IDisposable
    {
        private readonly IInternalPluginManager _pluginManager;

        private IStateInfo _archiveStateInfo;
        private IList<IStateInfo> _imageStateInfos;
        private IList<IList<PointMapping>> _pointMappings;

        private IArchiveState ArchiveState => _archiveStateInfo?.State as IArchiveState;

        public AnmcRessource(IInternalPluginManager pluginManager)
        {
            ContractAssertions.IsNotNull(pluginManager, nameof(pluginManager));

            _pluginManager = pluginManager;
        }

        public void Close()
        {
            _pluginManager.Close(_archiveStateInfo);
            foreach (var imageStateInfo in _imageStateInfos ?? Array.Empty<IStateInfo>())
                _pluginManager.Close(imageStateInfo);

            _archiveStateInfo = null;
            _imageStateInfos = null;
        }

        public void Dispose()
        {
            Close();
        }

        #region Load Methods

        public async Task<IList<AnmcNamedImageRessource>> Load(string file)
        {
            if (_archiveStateInfo != null)
                Close();

            // Load archive
            _archiveStateInfo = await LoadArchive(file);

            // Load images
            var archiveFileSystem = FileSystemFactory.CreateAfiFileSystem(ArchiveState, UPath.Root, _archiveStateInfo.StreamManager);
            _imageStateInfos = await LoadImages(archiveFileSystem);

            // Load point mappings
            var mappings = await LoadPvb(archiveFileSystem);
            _pointMappings = await LoadPbis(archiveFileSystem, mappings);

            // Load RES.bin
            var nameIndices = await LoadResBin(archiveFileSystem);

            // Create image providers
            var imageStates = _imageStateInfos.Select(x => x.State as IImageState).ToArray();
            var imageProviders = new ImageProvider[_imageStateInfos.Count];
            for (var i = 0; i < imageProviders.Length; i++)
                imageProviders[i] = new ImageProvider(new KanvasImage(imageStates[i], imageStates[i].Images[0]));

            // Create final image ressources
            var result = new List<AnmcNamedImageRessource>();
            for (var i = 0; i < Math.Min(_pointMappings.Count, nameIndices.Count); i++)
            {
                var nameIndex = nameIndices.First(x => x.Value.pbiIndex == i);
                var imageProvider = imageProviders[nameIndex.Value.imageIndex];

                var partIndex = 0;
                var imageRessources = new List<AnmcImageRessource>();
                for (var mappingIndex = 0; mappingIndex < _pointMappings[i].Count; mappingIndex += 6)
                {
                    var points = _pointMappings[i].Skip(mappingIndex).Take(6).ToArray();
                    var imageRessource = new AnmcImageRessource(imageProvider, points, $"{nameIndex.Key} Part {partIndex++}");

                    imageRessources.Add(imageRessource);
                }

                result.Add(new AnmcNamedImageRessource(nameIndex.Key, imageRessources));
            }

            return result;
        }

        private async Task<IStateInfo> LoadArchive(string file)
        {
            var loadResult = await _pluginManager.LoadFile(file);
            if (!loadResult.IsSuccessful)
                throw loadResult.Exception;

            return loadResult.LoadedState;
        }

        private async Task<IList<IStateInfo>> LoadImages(IFileSystem archiveFileSystem)
        {
            var imageStateInfos = new List<IStateInfo>();
            foreach (var file in archiveFileSystem.EnumeratePaths(UPath.Root, "*.xi", SearchOption.TopDirectoryOnly, SearchTarget.File))
            {
                var loadResult = await _pluginManager.LoadFile(archiveFileSystem, file);
                if (!loadResult.IsSuccessful)
                    throw loadResult.Exception;

                imageStateInfos.Add(loadResult.LoadedState);
            }

            return imageStateInfos;
        }

        private async Task<IList<PointMapping>> LoadPvb(IFileSystem archiveFileSystem)
        {
            var pvbPath = archiveFileSystem
                .EnumeratePaths(UPath.Root, "*.pvb", SearchOption.TopDirectoryOnly, SearchTarget.File)
                .First();

            var pvbStream = await archiveFileSystem.OpenFileAsync(pvbPath);
            using var br = new BinaryReaderX(pvbStream, true);

            if (br.ReadString(4) != "XPVB")
                throw new InvalidOperationException($"{pvbPath} is no valid pvb file.");

            // Get vertices
            br.BaseStream.Position += 4;
            var verticesOffset = br.ReadInt16();
            var verticesSize = br.ReadInt16();
            var verticesCount = br.ReadInt32();

            br.BaseStream.Position = verticesOffset;
            var verticesStream = Decompress(br.BaseStream);

            using var verticesBr = new BinaryReaderX(verticesStream);
            return verticesBr.ReadMultiple<PointMapping>(verticesCount);
        }

        private async Task<IList<IList<PointMapping>>> LoadPbis(IFileSystem fileSystem, IList<PointMapping> pointMappings)
        {
            var pbiPaths = fileSystem
                .EnumeratePaths(UPath.Root, "*.pbi", SearchOption.TopDirectoryOnly, SearchTarget.File);

            var result = new List<IList<PointMapping>>();
            foreach (var pbiPath in pbiPaths)
            {
                var pbiStream = await fileSystem.OpenFileAsync(pbiPath);
                using var br = new BinaryReaderX(pbiStream);

                if (br.ReadString(4) != "XPVI")
                    throw new InvalidOperationException($"{pbiPath} is no valid pbi file.");

                br.BaseStream.Position += 4;
                var pointCount = br.ReadInt32();

                var pointStream = Decompress(br.BaseStream);
                using var pointBr = new BinaryReaderX(pointStream);

                var pbiMappings = pointBr.ReadMultiple<short>(pointCount).Select(x => (PointMapping)pointMappings[x].Clone()).ToArray();
                result.Add(pbiMappings);
            }

            return result;
        }

        private async Task<IDictionary<string, (int imageIndex, int pbiIndex)>> LoadResBin(IFileSystem archiveFileSystem)
        {
            var resBinPath = archiveFileSystem
                .EnumeratePaths(UPath.Root, "*RES.bin", SearchOption.TopDirectoryOnly, SearchTarget.File)
                .First();
            var resFileStream = await archiveFileSystem.OpenFileAsync(resBinPath);
            using var br = new BinaryReaderX(resFileStream);

            // Check header magic
            if (br.PeekString() != "ANMC")
                throw new InvalidOperationException("RES.bin is no valid ANMC file.");

            // Read header
            var resHeader = br.ReadType<ResHeader>();

            // Read image tables
            var imageEntries = br.ReadMultiple<ResImageEntry>(resHeader.imageTables[0].entryCount);
            var imageAreas = br.ReadMultiple<ResImageArea>(resHeader.imageTables[1].entryCount);

            // Read pbi entries
            br.BaseStream.Position = resHeader.tableCluster2[1].offset << 2;
            var unkEntries = br.ReadMultiple<TableCluster2Table2>(resHeader.tableCluster2[1].entryCount);
            br.BaseStream.Position = resHeader.tableCluster2[3].offset << 2;
            var pbiEntries = br.ReadMultiple<ResPbiDimensionEntry>(resHeader.tableCluster2[3].entryCount);

            // Create string reader
            var stringOffset = resHeader.stringTablesOffset << 2;
            var stringStream = new SubStream(resFileStream, stringOffset, resFileStream.Length - stringOffset);
            using var stringBr = new BinaryReaderX(stringStream);

            // Create string connection to data
            var result = new Dictionary<string, (int, int)>();
            for (var i = 0; i < pbiEntries.Count; i++)
            {
                stringBr.BaseStream.Position = pbiEntries[i].stringPointer.offset;
                var pbiName = stringBr.ReadCStringSJIS();

                if (i >= unkEntries.Count)
                    continue;

                var imageAreaParent = unkEntries[i].imageAreaParent;
                var imageArea = imageAreas.First(x => x.stringPointer.crc32 == imageAreaParent);

                var imageEntryParent = imageArea.imageEntryParent;
                var imageEntry = imageEntries.First(x => x.stringPointer.crc32 == imageEntryParent);
                var imageEntryIndex = imageEntries.IndexOf(imageEntry);

                result[pbiName] = (imageEntryIndex, i);
            }

            return result;
        }

        #endregion

        #region Save methods

        public void Save(UPath savePath)
        {
            // 1. Create file system for archive
            var archiveFileSystem = FileSystemFactory.CreateAfiFileSystem(ArchiveState, UPath.Root, _archiveStateInfo.StreamManager);

            // 2. Save PVB and PBIs
            var distinctPointMappings = _pointMappings.SelectMany(x => x).Batch(6).Select(x => x.OrderBy(y => y.v).ThenBy(y => y.u)).SelectMany(x => x).Distinct().ToArray();
            SavePvb(archiveFileSystem, distinctPointMappings);
            SavePbis(archiveFileSystem, _pointMappings, distinctPointMappings);

            // 3. Save images
            foreach (var stateInfo in _imageStateInfos)
                _pluginManager.SaveFile(stateInfo);

            // 3. Save original archive
            _pluginManager.SaveFile(_archiveStateInfo, savePath);
        }

        private async void SavePvb(IFileSystem archiveFileSystem, IList<PointMapping> mappings)
        {
            var pvbPath = archiveFileSystem
                .EnumeratePaths(UPath.Root, "*.pvb", SearchOption.TopDirectoryOnly, SearchTarget.File)
                .First();
            var pvbStream = await archiveFileSystem.OpenFileAsync(pvbPath);

            var copy = new byte[0x10];
            pvbStream.Read(copy, 0, copy.Length);

            // Open output
            var output = new MemoryStream();
            await using var bw = new BinaryWriterX(output, true);

            // Write original data
            BinaryPrimitives.WriteInt16LittleEndian(copy.AsSpan(0xC), (short)mappings.Count);
            bw.Write(copy);

            var oldOffset = BinaryPrimitives.ReadInt16LittleEndian(copy.AsSpan(0x8));
            copy = new byte[oldOffset - 0x10];
            pvbStream.Read(copy, 0, copy.Length);
            bw.Write(copy);

            // Write compressed point mappings
            WriteMultipleCompressed(bw, mappings, Level5CompressionMethod.Lz10);

            // Set output to original file
            archiveFileSystem.SetFileData(pvbPath, output);
        }

        private async void SavePbis(IFileSystem archiveFileSystem, IList<IList<PointMapping>> mappings, IList<PointMapping> distinctPointMappings)
        {
            for (var pbiIndex = 0; pbiIndex < mappings.Count; pbiIndex++)
            {
                var pbiFile = $"{pbiIndex:000}.pbi";
                var pbiStream = await archiveFileSystem.OpenFileAsync(pbiFile);

                var copy = new byte[0x8];
                pbiStream.Read(copy, 0, copy.Length);

                var output = new MemoryStream();
                var bw = new BinaryWriterX(output, true);

                bw.Write(copy);
                bw.Write(mappings[pbiIndex].Count);

                var mappingIndices = mappings[pbiIndex]
                    .Select(x => (short)distinctPointMappings.IndexOf(x))
                    .ToArray();
                WriteMultipleCompressed(bw, mappingIndices, Level5CompressionMethod.Lz10);
                archiveFileSystem.SetFileData(pbiFile, output);

                bw.Dispose();
            }
        }

        #endregion

        private Stream Decompress(Stream input)
        {
            var output = new MemoryStream();
            Level5Compressor.Decompress(input, output);

            output.Position = 0;
            return output;
        }

        private void WriteMultipleCompressed<T>(BinaryWriterX bw, IList<T> list, Level5CompressionMethod comp)
        {
            if (!list.Any())
                return;

            var ms = new MemoryStream();
            using (var bwOut = new BinaryWriterX(ms, true))
                bwOut.WriteMultiple(list);

            var compressedStream = new MemoryStream();
            ms.Position = 0;
            Level5Compressor.Compress(ms, compressedStream, comp);

            compressedStream.Position = 0;
            compressedStream.CopyTo(bw.BaseStream);
        }
    }
}
