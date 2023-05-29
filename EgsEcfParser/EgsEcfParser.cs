using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using static EgsEcfParser.EcfDefinitionHandling;
using static EgsEcfParser.EcfFormatChecking;
using static EgsEcfParser.EcfKeyValueItem;
using static EgsEcfParser.EcfStructureTools;

namespace EgsEcfParser
{
    public static class EcfDefinitionHandling
    {
        public static string DefaultBaseFolder { get; set; } = "EcfFileDefinitions";
        public static string TemplateFileName { get; set; } = "VanillaEcfDefinition_BlocksConfig_DefaultTemplate.xml";

        private static List<FormatDefinition> Definitions { get; } = new List<FormatDefinition>();
        private static string ActualDefinitionsFolder { get; set; } = null;

        public enum EcfFileNewLineSymbols
        {
            Unknown,
            Lf,
            Cr,
            CrLf,
        }
        public enum ChangeableDefinitionChapters
        {
            BlockParameters,
            ParameterAttributes,
        }

        // public
        public static void ReloadDefinitions()
        {
            ActualDefinitionsFolder = null;
            XmlHandling.LoadDefinitionsFromFiles();
        }
        public static bool SaveItemToDefinitionFile(FormatDefinition definition, ChangeableDefinitionChapters chapter, ItemDefinition newBlockParameter)
        {
            return XmlHandling.SaveItemToDefinitionFile(definition, chapter, newBlockParameter);
        }
        public static List<string> GetGameModes()
        {
            XmlHandling.LoadDefinitionsFromFiles();
            return Definitions.Select(def => def.GameMode).Distinct().ToList();
        }
        public static List<FormatDefinition> GetSupportedFileTypes(string gameMode)
        {
            XmlHandling.LoadDefinitionsFromFiles();
            return Definitions.Where(def => def.GameMode.Equals(gameMode)).ToList();
        }
        public static FormatDefinition GetDefinition(string gameMode, string fileType)
        {
            XmlHandling.LoadDefinitionsFromFiles();
            return Definitions.FirstOrDefault(def => string.Equals(gameMode, def.GameMode) && string.Equals(fileType, def.FileType));
        }
        public static Encoding GetFileEncoding(string filePathAndName)
        {
            try
            {
                using (FileStream stream = new FileStream(filePathAndName, FileMode.Open, FileAccess.Read))
                {
                    byte[] bom = new byte[4];
                    stream.Read(bom, 0, 4);
                    try
                    {
                        if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
                        if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
                        if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0) return Encoding.UTF32; //UTF-32LE
                        if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
                        if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
                        if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return new UTF32Encoding(true, true);  //UTF-32BE
                    }
                    catch (Exception) { }
                    return new UTF8Encoding(false);
                }
            }
            catch (Exception ex)
            {
                throw new IOException(string.Format("File {0} encoding could not be loaded: {1}", filePathAndName, ex.Message));
            }
        }
        public static EcfFileNewLineSymbols GetNewLineSymbol(string filePathAndName)
        {
            try
            {
                StringBuilder charBuffer = new StringBuilder();
                using (StreamReader reader = new StreamReader(File.Open(filePathAndName, FileMode.Open, FileAccess.Read)))
                {
                    do
                    {
                        if (ReadChar(reader, out char buffer) != -1)
                        {
                            if (buffer == '\n')
                            {
                                return EcfFileNewLineSymbols.Lf;
                            }
                            else if (buffer == '\r')
                            {
                                if (ReadChar(reader, out buffer) != -1 && buffer == '\n')
                                {
                                    return EcfFileNewLineSymbols.CrLf;
                                }
                                else
                                {
                                    return EcfFileNewLineSymbols.Cr;
                                }
                            }
                        }
                    } while (!reader.EndOfStream);
                }
                return EcfFileNewLineSymbols.CrLf;
            }
            catch (Exception ex)
            {
                throw new IOException(string.Format("File {0} newline character could not be determined: {1}", filePathAndName, ex.Message));
            }
        }
        public static string GetNewLineChar(EcfFileNewLineSymbols newLineSymbol)
        {
            switch (newLineSymbol)
            {
                case EcfFileNewLineSymbols.Lf: return "\n";
                case EcfFileNewLineSymbols.Cr: return "\r";
                default: return "\r\n";
            }
        }

        // private
        public static List<ItemDefinition> FindDeprecatedItemDefinitions(EgsEcfFile file)
        {
            List<ItemDefinition> deprecatedItems = new List<ItemDefinition>();
            try
            {
                XmlHandling.LoadDefinitionsFromFiles();
                FormatDefinition definition = Definitions.FirstOrDefault(def => def.GameMode.Equals(file.Definition.GameMode) && def.FileType.Equals(file.Definition.FileType));
                List<ItemDefinition> rootBlockAttributes = definition.RootBlockAttributes.ToList();
                List<ItemDefinition> childBlockAttributes = definition.ChildBlockAttributes.ToList();
                List<ItemDefinition> blockParameters = definition.BlockParameters.ToList();
                List<ItemDefinition> parameterAttributes = definition.ParameterAttributes.ToList();
                foreach (EcfStructureItem item in file.GetDeepItemList<EcfStructureItem>())
                {
                    RemoveDefinitionsFromLists(item, rootBlockAttributes, childBlockAttributes, blockParameters, parameterAttributes);
                    if (rootBlockAttributes.Count == 0 && childBlockAttributes.Count == 0 && blockParameters.Count == 0 && parameterAttributes.Count == 0)
                    {
                        break;
                    }
                }
                deprecatedItems.AddRange(rootBlockAttributes);
                deprecatedItems.AddRange(childBlockAttributes);
                deprecatedItems.AddRange(blockParameters);
                deprecatedItems.AddRange(parameterAttributes);
            }
            catch (Exception) { }
            return deprecatedItems;
        }
        private static void RemoveDefinitionsFromLists(EcfStructureItem item,
            List<ItemDefinition> rootBlockAttributes, List<ItemDefinition> childBlockAttributes,
            List<ItemDefinition> blockParameters, List<ItemDefinition> parameterAttributes)
        {
            if (item is EcfParameter parameter)
            {
                if (parameterAttributes.Count > 0)
                {
                    parameterAttributes.RemoveAll(defAttr => parameter.Attributes.Any(attr => defAttr.Name.Equals(attr.Key)));
                }
                if (blockParameters.Count > 0)
                {
                    blockParameters.RemoveAll(defParam => defParam.Name.Equals(parameter.Key));
                }
            }
            else if (item is EcfBlock block)
            {
                if (block.IsRoot())
                {
                    if (rootBlockAttributes.Count > 0)
                    {
                        rootBlockAttributes.RemoveAll(defAttr => block.Attributes.Any(attr => defAttr.Name.Equals(attr.Key)));
                    }
                }
                else
                {
                    if (childBlockAttributes.Count > 0)
                    {
                        childBlockAttributes.RemoveAll(defAttr => block.Attributes.Any(attr => defAttr.Name.Equals(attr.Key)));
                    }
                }
            }
        }
        private static int ReadChar(StreamReader reader, out char c)
        {
            int i = reader.Read();
            if (i >= 0)
            {
                c = (char)i;
            }
            else
            {
                c = (char)0;
            }
            return i;
        }

        private static class XmlHandling
        {
            private static class XmlSettings
            {
                public static string FileNamePattern { get; } = "*.xml";

                public static string XChapterRoot { get; } = "Settings";

                public static string XChapterFileConfig { get; } = "Config";
                public static string XChapterContentLinking { get; } = "ContentLinking";
                public static string XChapterFormatting { get; } = "Formatting";
                public static string XChapterBlockTypePreMarks { get; } = "BlockTypePreMarks";
                public static string XChapterBlockTypePostMarks { get; } = "BlockTypePostMarks";
                public static string XChapterRootBlockTypes { get; } = "RootBlockTypes";
                public static string XChapterRootBlockAttributes { get; } = "RootBlockAttributes";
                public static string XChapterChildBlockTypes { get; } = "ChildBlockTypes";
                public static string XChapterChildBlockAttributes { get; } = "ChildBlockAttributes";
                public static string XChapterBlockParameters { get; } = "BlockParameters";
                public static string XChapterParameterAttributes { get; } = "ParameterAttributes";

                public static string XElementBlockIdAttribute { get; } = "BlockIdAttribute";
                public static string XElementBlockNameAttribute { get; } = "BlockNameAttribute";
                public static string XElementBlockReferenceSourceAttribute { get; } = "BlockReferenceSourceAttribute";
                public static string XElementBlockReferenceTargetAttribute { get; } = "BlockReferenceTargetAttribute";
                public static string XElementDefinesItems { get; } = "DefinesItems";
                public static string XElementDefinesTemplates { get; } = "DefinesTemplates";
                public static string XElementDefinesBuildBlocks { get; } = "DefinesBuildBlocks";
                public static string XElementDefinesBuildBlockGroups { get; } = "DefinesBuildBlockGroups";
                public static string XElementDefinesGlobalMacros { get; } = "DefinesGlobalMacros";
                public static string XElementDefinesGlobalMacroUsers { get; } = "DefinesGlobalMacroUsers";
                public static string XElementSingleLineCommentStart { get; } = "SingleLineCommentStart";
                public static string XElementMultiLineCommentPair { get; } = "MultiLineCommentPair";
                public static string XElementBlockIdentifierPair { get; } = "BlockIdentifierPair";
                public static string XElementOuterTrimmingChar { get; } = "OuterTrimmingChar";
                public static string XElementItemSeperator { get; } = "ItemSeperator";
                public static string XElementItemValueSeperator { get; } = "ItemValueSeperator";
                public static string XElementValueSeperator { get; } = "ValueSeperator";
                public static string XElementValueGroupSeperator { get; } = "ValueGroupSeperator";
                public static string XElementValueFractionalSeperator { get; } = "ValueFractionalSeperator";
                public static string XElementMagicSpacer { get; } = "MagicSpacer";
                public static string XElementEscapeIdentifierPair { get; } = "EscapeIdentifierPair";
                public static string XElementParameter { get; } = "Param";

                public static string XAttributeMode { get; } = "mode";
                public static string XAttributeType { get; } = "type";
                public static string XAttributeValue { get; } = "value";
                public static string XAttributeOpener { get; } = "opener";
                public static string XAttributeCloser { get; } = "closer";
                public static string XAttributeName { get; } = "name";
                public static string XAttributeIsOptional { get; } = "optional";
                public static string XAttributeHasValue { get; } = "hasValue";
                public static string XAttributeIsAllowingBlank { get; } = "allowBlank";
                public static string XAttributeIsForceEscaped { get; } = "forceEscape";
                public static string XAttributeInfo { get; } = "info";
            }
            
            private static XmlDocument XmlDoc { get; } = new XmlDocument();

            // public
            public static void LoadDefinitionsFromFiles()
            {
                if (string.IsNullOrEmpty(ActualDefinitionsFolder) || !ActualDefinitionsFolder.Equals(DefaultBaseFolder))
                {
                    Definitions.Clear();
                    ActualDefinitionsFolder = null;
                    try
                    {
                        if (Directory.Exists(DefaultBaseFolder))
                        {
                            foreach (string filePathAndName in Directory.GetFiles(DefaultBaseFolder, XmlSettings.FileNamePattern, SearchOption.AllDirectories))
                            {
                                try
                                {
                                    ReadDefinitionFile(filePathAndName);
                                }
                                catch (Exception ex)
                                {
                                    throw new IOException(string.Format("Settings file '{0}' could not be loaded: {1}", filePathAndName, ex.Message));
                                }
                            }
                            ActualDefinitionsFolder = DefaultBaseFolder;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new IOException(string.Format("Settings files could not be found: {0}", ex.Message));
                    }
                    if (Definitions.Count == 0)
                    {
                        string filePathAndName = Path.Combine(DefaultBaseFolder, TemplateFileName);
                        try
                        {
                            Directory.CreateDirectory(DefaultBaseFolder);
                            CreateXmlTemplate(filePathAndName);
                            ReadDefinitionFile(filePathAndName);
                        }
                        catch (Exception ex)
                        {
                            throw new IOException(string.Format("Template settings file '{0}' could not be loaded: {1}", filePathAndName, ex.Message));
                        }
                        ActualDefinitionsFolder = DefaultBaseFolder;
                    }
                }
            }
            public static bool SaveItemToDefinitionFile(FormatDefinition definition, ChangeableDefinitionChapters chapter, ItemDefinition parameterItem)
            {
                switch (chapter)
                {
                    case ChangeableDefinitionChapters.BlockParameters: 
                        return SaveItemToDefinitionFile(definition, XmlSettings.XChapterBlockParameters, parameterItem);
                    case ChangeableDefinitionChapters.ParameterAttributes:
                        return SaveItemToDefinitionFile(definition, XmlSettings.XChapterParameterAttributes, parameterItem);
                    default: throw new ArgumentException(string.Format("No SaveItemToDefinitionFile Method attached to {0} .... That shouldn't happen!", chapter.ToString()));
                }
            }

            // private
            private static void ReadDefinitionFile(string filePathAndName)
            {
                XmlDoc.Load(filePathAndName);
                foreach (XmlNode configNode in XmlDoc.SelectNodes(string.Format("//{0}", XmlSettings.XChapterFileConfig)))
                {
                    string gameMode = configNode.Attributes?.GetNamedItem(XmlSettings.XAttributeMode)?.Value;
                    string fileType = configNode.Attributes?.GetNamedItem(XmlSettings.XAttributeType)?.Value;
                    if (string.IsNullOrEmpty(gameMode) || string.IsNullOrEmpty(fileType) || 
                        Definitions.Any(def => def.GameMode.Equals(gameMode) && def.FileType.Equals(fileType)))
                    {
                        continue;
                    }
                    Definitions.Add(BuildFormatDefinition(configNode, filePathAndName, gameMode, fileType));
                }
            }
            private static FormatDefinition BuildFormatDefinition(XmlNode configNode, string filePathAndName, string gameMode, string fileType)
            {
                XmlNode contentLinkingNode = configNode.SelectSingleNode(XmlSettings.XChapterContentLinking);
                if (contentLinkingNode == null) { throw new ArgumentException(string.Format("Chapter {0} not found", XmlSettings.XChapterContentLinking)); }
                string blockIdAttribute = RepairXmlControlLiterals(contentLinkingNode.SelectSingleNode(XmlSettings.XElementBlockIdAttribute)?
                    .Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);
                string blockNameAttribute = RepairXmlControlLiterals(contentLinkingNode.SelectSingleNode(XmlSettings.XElementBlockNameAttribute)?
                    .Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);
                string blockReferenceSourceAttribute = RepairXmlControlLiterals(contentLinkingNode.SelectSingleNode(XmlSettings.XElementBlockReferenceSourceAttribute)?
                    .Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);
                string blockReferenceTargetAttribute = RepairXmlControlLiterals(contentLinkingNode.SelectSingleNode(XmlSettings.XElementBlockReferenceTargetAttribute)?
                    .Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);
                
                bool isDefiningItems = Convert.ToBoolean(contentLinkingNode.SelectSingleNode(XmlSettings.XElementDefinesItems)?
                    .Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);
                bool isDefiningTemplates = Convert.ToBoolean(contentLinkingNode.SelectSingleNode(XmlSettings.XElementDefinesTemplates)?
                    .Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);
                bool isDefiningBuildBlocks = Convert.ToBoolean(contentLinkingNode.SelectSingleNode(XmlSettings.XElementDefinesBuildBlocks)?
                    .Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);
                bool isDefiningBuildBlockGroups = Convert.ToBoolean(contentLinkingNode.SelectSingleNode(XmlSettings.XElementDefinesBuildBlockGroups)?
                    .Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);
                bool isDefiningGlobalMacros = Convert.ToBoolean(contentLinkingNode.SelectSingleNode(XmlSettings.XElementDefinesGlobalMacros)?
                    .Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);
                bool isDefiningGlobalMacroUsers = Convert.ToBoolean(contentLinkingNode.SelectSingleNode(XmlSettings.XElementDefinesGlobalMacroUsers)?
                    .Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);

                XmlNode formatterNode = configNode.SelectSingleNode(XmlSettings.XChapterFormatting);
                if (formatterNode == null) { throw new ArgumentException(string.Format("Chapter {0} not found", XmlSettings.XChapterFormatting)); }

                List<string> singleLineCommentStarts = BuildStringList(formatterNode, XmlSettings.XElementSingleLineCommentStart);
                if (singleLineCommentStarts.Count < 1) { throw new ArgumentException(string.Format("No valid {0} found", XmlSettings.XElementSingleLineCommentStart)); }
                List<StringPairDefinition> multiLineCommentPairs = BuildStringPairList(formatterNode, XmlSettings.XElementMultiLineCommentPair);
                List<StringPairDefinition> blockPairs = BuildStringPairList(formatterNode, XmlSettings.XElementBlockIdentifierPair);
                if (blockPairs.Count < 1) { throw new ArgumentException(string.Format("No valid {0} found", XmlSettings.XElementBlockIdentifierPair)); }
                List<StringPairDefinition> escapeIdentifierPairs = BuildStringPairList(formatterNode, XmlSettings.XElementEscapeIdentifierPair);
                if (escapeIdentifierPairs.Count < 1) { throw new ArgumentException(string.Format("No valid {0} found", XmlSettings.XElementEscapeIdentifierPair)); }
                List<string> outerTrimmingPhrases = BuildStringList(formatterNode, XmlSettings.XElementOuterTrimmingChar);

                string itemSeperator = RepairXmlControlLiterals(formatterNode.SelectSingleNode(XmlSettings.XElementItemSeperator)?
                    .Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);
                string itemValueSeperator = RepairXmlControlLiterals(formatterNode.SelectSingleNode(XmlSettings.XElementItemValueSeperator)?
                    .Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);
                string valueSeperator = RepairXmlControlLiterals(formatterNode.SelectSingleNode(XmlSettings.XElementValueSeperator)?
                    .Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);
                string valueGroupSeperator = RepairXmlControlLiterals(formatterNode.SelectSingleNode(XmlSettings.XElementValueGroupSeperator)?
                    .Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);
                string valueFractionalSeperator = RepairXmlControlLiterals(formatterNode.SelectSingleNode(XmlSettings.XElementValueFractionalSeperator)?
                    .Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);
                string magicSpacer = RepairXmlControlLiterals(formatterNode.SelectSingleNode(XmlSettings.XElementMagicSpacer)?
                    .Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);

                if (!IsKeyValid(itemSeperator)) { throw new ArgumentException(string.Format("Element {0} not valid", XmlSettings.XElementItemSeperator)); }
                if (!IsKeyValid(itemValueSeperator)) { throw new ArgumentException(string.Format("Element {0} not valid", XmlSettings.XElementItemValueSeperator)); }
                if (!IsKeyValid(valueSeperator)) { throw new ArgumentException(string.Format("Element {0} not valid", XmlSettings.XElementValueSeperator)); }
                if (!IsKeyValid(valueGroupSeperator)) { throw new ArgumentException(string.Format("Element {0} not valid", XmlSettings.XElementValueGroupSeperator)); }
                if (!IsKeyValid(valueFractionalSeperator)) { throw new ArgumentException(string.Format("Element {0} not valid", XmlSettings.XElementValueFractionalSeperator)); }
                if (!IsKeyValid(magicSpacer)) { throw new ArgumentException(string.Format("Element {0} not valid", XmlSettings.XElementMagicSpacer)); }

                List<BlockValueDefinition> blockTypePreMarks = BuildMarkList(configNode, XmlSettings.XChapterBlockTypePreMarks);
                List<BlockValueDefinition> blockTypePostMarks = BuildMarkList(configNode, XmlSettings.XChapterBlockTypePostMarks);
                List<BlockValueDefinition> rootBlockTypes = BuildBlockTypeList(configNode, XmlSettings.XChapterRootBlockTypes);
                List<ItemDefinition> rootBlockAttributes = BuildItemList(configNode, XmlSettings.XChapterRootBlockAttributes);
                List<BlockValueDefinition> childBlockTypes = BuildBlockTypeList(configNode, XmlSettings.XChapterChildBlockTypes);
                List<ItemDefinition> childBlockAttributes = BuildItemList(configNode, XmlSettings.XChapterChildBlockAttributes);
                List<ItemDefinition> blockParameters = BuildItemList(configNode, XmlSettings.XChapterBlockParameters);
                List<ItemDefinition> parameterAttributes = BuildItemList(configNode, XmlSettings.XChapterParameterAttributes);

                return new FormatDefinition(filePathAndName, gameMode, fileType,
                    blockIdAttribute, blockNameAttribute, blockReferenceSourceAttribute, blockReferenceTargetAttribute,
                    isDefiningItems, isDefiningTemplates, isDefiningBuildBlocks, isDefiningBuildBlockGroups,
                    isDefiningGlobalMacros, isDefiningGlobalMacroUsers,
                    singleLineCommentStarts, multiLineCommentPairs,
                    blockPairs, escapeIdentifierPairs, outerTrimmingPhrases,
                    itemSeperator, itemValueSeperator, valueSeperator,
                    valueGroupSeperator, valueFractionalSeperator, magicSpacer,
                    blockTypePreMarks, blockTypePostMarks,
                    rootBlockTypes, rootBlockAttributes,
                    childBlockTypes, childBlockAttributes,
                    blockParameters, parameterAttributes
                );
            }
            private static string RepairXmlControlLiterals(string xmlString)
            {
                return xmlString?.Replace("\\t", "\t").Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\v", "\v");
            }
            private static bool SaveItemToDefinitionFile(FormatDefinition definition, string xChapter, ItemDefinition parameterItem)
            {
                XmlDoc.Load(definition.FilePathAndName);
                bool somethingCreated = false;
                foreach (XmlNode configNode in XmlDoc.SelectNodes(string.Format("//{0}", XmlSettings.XChapterFileConfig)))
                {
                    if (!(configNode.SelectSingleNode(xChapter) is XmlNode chapter))
                    {
                        chapter = CreateXmlChapter(XmlDoc, xChapter);
                        configNode.InsertAfter(chapter, configNode.LastChild);
                        somethingCreated = true;
                    }
                    if (!(chapter.SelectNodes(XmlSettings.XElementParameter).Cast<XmlNode>().FirstOrDefault(param =>
                        string.Equals(param.Attributes?.GetNamedItem(XmlSettings.XAttributeName)?.Value, parameterItem.Name)) is XmlNode parameter))
                    {
                        parameter = CreateXmlParameterItem(XmlDoc, parameterItem);
                        chapter.InsertAfter(parameter, chapter.LastChild);
                        somethingCreated = true;
                    }
                }
                if (somethingCreated) { XmlDoc.Save(definition.FilePathAndName); }
                return somethingCreated;
            }

            private static void CreateXmlTemplate(string filePathAndName)
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                };
                using (XmlWriter writer = XmlWriter.Create(filePathAndName, settings))
                {
                    writer.WriteStartElement(XmlSettings.XChapterRoot);
                    writer.WriteComment("Copy Config struct to define new file types if needed");
                    writer.WriteStartElement(XmlSettings.XChapterFileConfig);
                    writer.WriteAttributeString(XmlSettings.XAttributeMode, "Vanilla");
                    writer.WriteAttributeString(XmlSettings.XAttributeType, "BlocksConfig");
                    // Content Linking Settings
                    writer.WriteComment("Content Linking Settings");
                    {
                        writer.WriteStartElement(XmlSettings.XChapterContentLinking);
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementBlockIdAttribute, "Id");
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementBlockNameAttribute, "Name");
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementBlockReferenceSourceAttribute, "Ref");
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementBlockReferenceTargetAttribute, "Name");
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementDefinesItems, "true");
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementDefinesTemplates, "false");
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementDefinesBuildBlocks, "true");
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementDefinesBuildBlockGroups, "false");
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementDefinesGlobalMacros, "false");
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementDefinesGlobalMacroUsers, "true");
                        writer.WriteEndElement();
                    }
                    // Formatting
                    writer.WriteComment("Ecf Syntax Format Settings");
                    {
                        writer.WriteStartElement(XmlSettings.XChapterFormatting);
                        writer.WriteComment("Copy Parameters if more needed");
                        CreateXmlPairValueItem(writer, XmlSettings.XElementMultiLineCommentPair, "/*", "*/");
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementOuterTrimmingChar, " ");
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementOuterTrimmingChar, "\\t");
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementOuterTrimmingChar, "\\v");
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementOuterTrimmingChar, "\\r");
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementOuterTrimmingChar, "\\n");
                        writer.WriteComment("Specific Parameters");
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementItemSeperator, ",");
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementItemValueSeperator, ":");
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementValueSeperator, ",");
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementValueGroupSeperator, ";");
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementValueFractionalSeperator, ".");
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementMagicSpacer, " ");
                        writer.WriteComment("Copy Parameter if more needed, First is used at file write");
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementSingleLineCommentStart, "#");
                        CreateXmlPairValueItem(writer, XmlSettings.XElementBlockIdentifierPair, "{", "}");
                        CreateXmlPairValueItem(writer, XmlSettings.XElementEscapeIdentifierPair, "\"", "\"");
                        writer.WriteEndElement();
                    }
                    // premarks
                    writer.WriteComment("File Specific Syntax Settings, Add more child-params if needed");
                    {
                        writer.WriteStartElement(XmlSettings.XChapterBlockTypePreMarks);
                        CreateXmlOptionalValueItem(writer, "+", true);
                        writer.WriteEndElement();
                        writer.WriteStartElement(XmlSettings.XChapterBlockTypePostMarks);
                        CreateXmlOptionalValueItem(writer, " ", false);
                        writer.WriteEndElement();
                    }
                    // Root block types
                    {
                        writer.WriteStartElement(XmlSettings.XChapterRootBlockTypes);
                        CreateXmlTypeItem(writer, "Block", false);
                        writer.WriteEndElement();
                    }
                    // root block Attributes
                    {
                        writer.WriteStartElement(XmlSettings.XChapterRootBlockAttributes); ;
                        CreateXmlParameterItem(writer, "Id", true, true, false, false, "");
                        CreateXmlParameterItem(writer, "Name", false, true, false, false, "");
                        CreateXmlParameterItem(writer, "Ref", true, true, false, false, "");
                        writer.WriteEndElement();
                    }
                    // Child block types
                    {
                        writer.WriteStartElement(XmlSettings.XChapterChildBlockTypes);
                        CreateXmlTypeItem(writer, "Child", false);
                        writer.WriteEndElement();
                    }
                    // child block Attributes
                    {
                        writer.WriteStartElement(XmlSettings.XChapterChildBlockAttributes);
                        CreateXmlParameterItem(writer, "DropOnDestroy", true, false, false, false, "");
                        writer.WriteEndElement();
                    }
                    // block parameters
                    {
                        writer.WriteStartElement(XmlSettings.XChapterBlockParameters);
                        CreateXmlParameterItem(writer, "Material", true, true, false, false, "");
                        CreateXmlParameterItem(writer, "Shape", true, true, false, false, "");
                        CreateXmlParameterItem(writer, "Mesh", true, true, false, false, "");
                        writer.WriteEndElement();
                    }
                    // parameter Attributes
                    {
                        writer.WriteStartElement(XmlSettings.XChapterParameterAttributes);
                        CreateXmlParameterItem(writer, "type", true, true, false, false, "");
                        CreateXmlParameterItem(writer, "display", true, true, false, false, "");
                        CreateXmlParameterItem(writer, "formatter", true, true, false, false, "");
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
            }
            private static void CreateXmlPairValueItem(XmlWriter writer, string name, string opener, string closer)
            {
                writer.WriteStartElement(name);
                writer.WriteAttributeString(XmlSettings.XAttributeOpener, opener);
                writer.WriteAttributeString(XmlSettings.XAttributeCloser, closer);
                writer.WriteEndElement();
            }
            private static void CreateXmlSpecificValueItem(XmlWriter writer, string name, string value)
            {
                writer.WriteStartElement(name);
                writer.WriteAttributeString(XmlSettings.XAttributeValue, value);
                writer.WriteEndElement();
            }
            private static void CreateXmlOptionalValueItem(XmlWriter writer, string value, bool isOptional)
            {
                writer.WriteStartElement(XmlSettings.XElementParameter);
                writer.WriteAttributeString(XmlSettings.XAttributeValue, value);
                writer.WriteAttributeString(XmlSettings.XAttributeIsOptional, isOptional.ToString().ToLower());
                writer.WriteEndElement();
            }
            private static void CreateXmlTypeItem(XmlWriter writer, string name, bool isOptional)
            {
                writer.WriteStartElement(XmlSettings.XElementParameter);
                writer.WriteAttributeString(XmlSettings.XAttributeName, name);
                writer.WriteAttributeString(XmlSettings.XAttributeIsOptional, isOptional.ToString().ToLower());
                writer.WriteEndElement();
            }
            private static void CreateXmlParameterItem(XmlWriter writer, string name, bool isOptional, bool hasValue, bool isAllowingBlank, bool isForceEscaped, string info)
            {
                writer.WriteStartElement(XmlSettings.XElementParameter);
                writer.WriteAttributeString(XmlSettings.XAttributeName, name);
                writer.WriteAttributeString(XmlSettings.XAttributeIsOptional, isOptional.ToString().ToLower());
                writer.WriteAttributeString(XmlSettings.XAttributeHasValue, hasValue.ToString().ToLower());
                writer.WriteAttributeString(XmlSettings.XAttributeIsAllowingBlank, isAllowingBlank.ToString().ToLower());
                writer.WriteAttributeString(XmlSettings.XAttributeIsForceEscaped, isForceEscaped.ToString().ToLower());
                writer.WriteAttributeString(XmlSettings.XAttributeInfo, info);
                writer.WriteEndElement();
            }
            private static XmlNode CreateXmlChapter(XmlDocument xmlDoc, string xChapter)
            {
                XmlNode chapter = xmlDoc.CreateNode(XmlNodeType.Element, xChapter, "");
                return chapter;
            }
            private static XmlNode CreateXmlParameterItem(XmlDocument xmlDoc, ItemDefinition newItem)
            {
                XmlNode parameter = xmlDoc.CreateNode(XmlNodeType.Element, XmlSettings.XElementParameter, "");
                parameter.Attributes.Append(CreateXmlAttribute(xmlDoc, XmlSettings.XAttributeName, newItem.Name));
                parameter.Attributes.Append(CreateXmlAttribute(xmlDoc, XmlSettings.XAttributeIsOptional, newItem.IsOptional.ToString().ToLower()));
                parameter.Attributes.Append(CreateXmlAttribute(xmlDoc, XmlSettings.XAttributeHasValue, newItem.HasValue.ToString().ToLower()));
                parameter.Attributes.Append(CreateXmlAttribute(xmlDoc, XmlSettings.XAttributeIsAllowingBlank, newItem.IsAllowingBlank.ToString().ToLower()));
                parameter.Attributes.Append(CreateXmlAttribute(xmlDoc, XmlSettings.XAttributeIsForceEscaped, newItem.IsForceEscaped.ToString().ToLower()));
                parameter.Attributes.Append(CreateXmlAttribute(xmlDoc, XmlSettings.XAttributeInfo, newItem.Info));
                return parameter;
            }
            private static XmlAttribute CreateXmlAttribute(XmlDocument xmlDoc, string name, string value)
            {
                XmlAttribute attribute = xmlDoc.CreateAttribute(name);
                attribute.Value = value;
                return attribute;
            }

            private static List<string> BuildStringList(XmlNode formatterNode, string xElement)
            {
                List<string> strings = new List<string>();
                foreach (XmlNode node in formatterNode.SelectNodes(xElement))
                {
                    string data = RepairXmlControlLiterals(node.Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);
                    if (!IsKeyValid(data)) { throw new ArgumentException(string.Format("'{0}' is not a valid string parameter", data)); }
                    strings.Add(data);
                }
                return strings;
            }
            private static List<StringPairDefinition> BuildStringPairList(XmlNode formatterNode, string xElement)
            {
                List<StringPairDefinition> pairs = new List<StringPairDefinition>();
                foreach (XmlNode node in formatterNode.SelectNodes(xElement))
                {
                    pairs.Add(new StringPairDefinition(
                        RepairXmlControlLiterals(node.Attributes?.GetNamedItem(XmlSettings.XAttributeOpener)?.Value),
                        RepairXmlControlLiterals(node.Attributes?.GetNamedItem(XmlSettings.XAttributeCloser)?.Value)
                        ));
                }
                return pairs;
            }
            private static List<BlockValueDefinition> BuildMarkList(XmlNode fileNode, string xChapter)
            {
                List<BlockValueDefinition> preMarks = new List<BlockValueDefinition>();
                foreach (XmlNode node in fileNode.SelectSingleNode(xChapter)?.SelectNodes(XmlSettings.XElementParameter))
                {
                    preMarks.Add(
                        new BlockValueDefinition(
                            RepairXmlControlLiterals(node.Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value),
                            node.Attributes?.GetNamedItem(XmlSettings.XAttributeIsOptional)?.Value,
                            false));
                }
                return preMarks;
            }
            private static List<BlockValueDefinition> BuildBlockTypeList(XmlNode fileNode, string xChapter)
            {
                List<BlockValueDefinition> blockTypes = new List<BlockValueDefinition>();
                foreach (XmlNode node in fileNode.SelectSingleNode(xChapter)?.SelectNodes(XmlSettings.XElementParameter))
                {
                    blockTypes.Add(new BlockValueDefinition(
                        node.Attributes?.GetNamedItem(XmlSettings.XAttributeName)?.Value,
                        node.Attributes?.GetNamedItem(XmlSettings.XAttributeIsOptional)?.Value
                        ));
                }
                return blockTypes;
            }
            private static List<ItemDefinition> BuildItemList(XmlNode fileNode, string xChapter)
            {
                List<ItemDefinition> parameters = new List<ItemDefinition>();
                foreach (XmlNode node in fileNode.SelectSingleNode(xChapter)?.SelectNodes(XmlSettings.XElementParameter))
                {
                    parameters.Add(new ItemDefinition(
                        node.Attributes?.GetNamedItem(XmlSettings.XAttributeName)?.Value,
                        node.Attributes?.GetNamedItem(XmlSettings.XAttributeIsOptional)?.Value,
                        node.Attributes?.GetNamedItem(XmlSettings.XAttributeHasValue)?.Value,
                        node.Attributes?.GetNamedItem(XmlSettings.XAttributeIsAllowingBlank)?.Value,
                        node.Attributes?.GetNamedItem(XmlSettings.XAttributeIsForceEscaped)?.Value,
                        node.Attributes?.GetNamedItem(XmlSettings.XAttributeInfo)?.Value
                        ));
                }
                return parameters;
            }
        }
    }
    public static class EcfFormatChecking
    {
        public static bool IsKeyValid(string key)
        {
            return !string.IsNullOrEmpty(key);
        }

        public static List<EcfError> CheckBlockPreMark(string dataType, ReadOnlyCollection<BlockValueDefinition> definedDataTypes, EcfErrorGroups errorGroup)
        {
            return CheckBlockDataType(dataType, definedDataTypes, errorGroup, EcfErrors.BlockPreMarkMissing, EcfErrors.BlockPreMarkUnknown);
        }
        public static List<EcfError> CheckBlockDataType(string dataType, ReadOnlyCollection<BlockValueDefinition> definedDataTypes, EcfErrorGroups errorGroup)
        {
            return CheckBlockDataType(dataType, definedDataTypes, errorGroup, EcfErrors.BlockDataTypeMissing, EcfErrors.BlockDataTypeUnknown);
        }
        public static List<EcfError> CheckBlockPostMark(string dataType, ReadOnlyCollection<BlockValueDefinition> definedDataTypes, EcfErrorGroups errorGroup)
        {
            return CheckBlockDataType(dataType, definedDataTypes, errorGroup, EcfErrors.BlockPostMarkMissing, EcfErrors.BlockPostMarkUnknown);
        }
        public static List<EcfError> CheckBlockDataType(string dataType, ReadOnlyCollection<BlockValueDefinition> definedDataTypes,
            EcfErrorGroups errorGroup, EcfErrors missingError, EcfErrors unknownError)
        {
            List<EcfError> errors = new List<EcfError>();
            List<BlockValueDefinition> mandatoryDataTypes = definedDataTypes.Where(type => !type.IsOptional).ToList();
            if (mandatoryDataTypes.Count > 0 && !mandatoryDataTypes.Any(type => type.Value.Equals(dataType)))
            {
                errors.Add(new EcfError(errorGroup, missingError, string.Format("found '{0}', expected: '{1}'", dataType,
                    string.Join(", ", mandatoryDataTypes.Select(type => type.Value).ToArray()))));
            }
            else if (dataType != null && !definedDataTypes.Any(type => type.Value.Equals(dataType)))
            {
                errors.Add(new EcfError(errorGroup, unknownError, dataType));
            }
            return errors;
        }
        public static List<EcfError> CheckBlockUniqueness(EcfBlock block, List<EcfBlock> blockList, EcfErrorGroups errorGroup)
        {
            string idValue = block.GetId();
            string refTargetValue = block.GetRefTarget();
            bool isReferenced = refTargetValue != null && blockList.Any(listedBlock => refTargetValue.Equals(listedBlock.GetRefSource()));
            return blockList.Where(listedBlock => !listedBlock.Equals(block) &&
                ((idValue?.Equals(listedBlock.GetId()) ?? false) || (isReferenced && refTargetValue.Equals(listedBlock.GetRefTarget()))))
                .Select(listedBlock => new EcfError(errorGroup, EcfErrors.BlockIdNotUnique, listedBlock.BuildRootId())).ToList();
        }
        public static EcfError CheckBlockReferenceValid(EcfBlock block, List<EcfBlock> blockList, out EcfBlock inheriter, EcfErrorGroups errorGroup)
        {
            string referenceValue = block.GetRefSource();
            if (referenceValue == null) {
                inheriter = null;
                return null;
            }
            inheriter = blockList.FirstOrDefault(parentBlock => parentBlock.GetRefTarget()?.Equals(referenceValue) ?? false);
            if (inheriter == null)
            {
                return new EcfError(errorGroup, EcfErrors.BlockInheritorMissing, referenceValue);
            }
            return null;
        }
        public static List<ItemDefinition> CheckItemsMissing<T>(List<T> parameters, ReadOnlyCollection<ItemDefinition> definedParameters) where T : EcfKeyValueItem
        {
            return definedParameters?.Where(defParam => !defParam.IsOptional && !parameters.Any(param => param.Key.Equals(defParam.Name))).ToList();
        }
        public static List<T> CheckItemsDoubled<T>(List<T> items) where T : EcfKeyValueItem
        {
            return items.Except(items.Distinct(KeyItemComparer)).Cast<T>().ToList();
        }
        public static EcfError CheckItemUnknown(ReadOnlyCollection<ItemDefinition> definition, string key, 
            KeyValueItemTypes? itemType, out ItemDefinition itemDefinition, EcfErrorGroups errorGroup)
        {
            itemDefinition = definition?.FirstOrDefault(defParam => defParam.Name.Equals(key));
            if (itemDefinition != null) { return null; }

            switch (itemType)
            {
                case KeyValueItemTypes.Parameter: return new EcfError(errorGroup, EcfErrors.ParameterUnknown, key);
                case KeyValueItemTypes.Attribute: return new EcfError(errorGroup, EcfErrors.AttributeUnknown, key);
                default: return new EcfError(errorGroup, EcfErrors.Unknown, key);
            }
        }
        public static List<EcfError> CheckParametersValid(List<EcfParameter> parameters, ReadOnlyCollection<ItemDefinition> definedParameters, EcfErrorGroups errorGroup)
        {
            List<EcfError> errors = new List<EcfError>();
            CheckItemsMissing(parameters, definedParameters).ForEach(missingParam =>
            {
                errors.Add(new EcfError(errorGroup, EcfErrors.ParameterMissing, missingParam.Name));
            });
            CheckItemsDoubled(parameters).ForEach(doubledParam =>
            {
                errors.Add(new EcfError(errorGroup, EcfErrors.ParameterDoubled, doubledParam.Key));
            });
            return errors;
        }
        public static List<EcfError> CheckAttributesValid(List<EcfAttribute> attributes, ReadOnlyCollection<ItemDefinition> definedAttributes, EcfErrorGroups errorGroup)
        {
            List<EcfError> errors = new List<EcfError>();
            CheckItemsMissing(attributes, definedAttributes)?.ForEach(missingAttr =>
            {
                errors.Add(new EcfError(errorGroup, EcfErrors.AttributeMissing, missingAttr.Name));
            });
            CheckItemsDoubled(attributes).ForEach(doubledAttr =>
            {
                errors.Add(new EcfError(errorGroup, EcfErrors.AttributeDoubled, doubledAttr.Key));
            });
            return errors;
        }
        public static List<EcfError> CheckValuesValid(List<EcfValueGroup> groups, ItemDefinition itemDef, FormatDefinition formatDef, EcfErrorGroups errorGroup)
        {
            List<EcfError> errors = new List<EcfError>();
            if (groups == null || !groups.Any(group => group.Values.Count > 0))
            {
                if (itemDef?.HasValue ?? true)
                {
                    errors.Add(new EcfError(errorGroup, EcfErrors.ValueGroupEmpty, "Not at least one value present"));
                }
            }
            else
            {
                int groupCount = 1;
                int valueCount = 1;
                foreach (EcfValueGroup group in groups)
                {
                    foreach (string value in group.Values)
                    {
                        errors.AddRange(CheckValueValid(value, itemDef, formatDef, string.Format("group: {0}, value: {1}", groupCount, valueCount), errorGroup));
                        valueCount++;
                    }
                    groupCount++;
                    valueCount = 1;
                }
            }
            return errors;
        }
        public static List<EcfError> CheckValueValid(string value, ItemDefinition itemDef, FormatDefinition formatDef, string errorInfo, EcfErrorGroups errorGroup)
        {
            List<EcfError> errors = new List<EcfError>();
            if (value == null)
            {
                errors.Add(new EcfError(errorGroup, EcfErrors.ValueNull, errorInfo ?? "Value null"));
            }
            else if (!(itemDef?.IsAllowingBlank ?? false) && value.Equals(string.Empty))
            {
                errors.Add(new EcfError(errorGroup, EcfErrors.ValueEmpty, errorInfo ?? "Value empty"));
            }
            else
            {
                List<string> foundProhibitedPhrases = formatDef?.ProhibitedValuePhrases.Where(phrase => value.Contains(phrase)).ToList();
                foundProhibitedPhrases?.ForEach(phrase =>
                {
                    errors.Add(new EcfError(errorGroup, EcfErrors.ValueContainsProhibitedPhrases, phrase));
                });
            }
            return errors;
        }
        public static List<EcfError> CheckParameterInterFileDependencies(EcfDependencyParameters depenParams,
            List<EgsEcfFile> filesToCheck, List<EcfParameter> parametersToCheck, EcfErrorGroups errorGroup)
        {
            return parametersToCheck.SelectMany(parameter => CheckParameterInterFileDependencies(depenParams, filesToCheck, parameter, errorGroup)).ToList();
        }
        public static List<EcfError> CheckParameterInterFileDependencies(EcfDependencyParameters depenParams,
            List<EgsEcfFile> filesToCheck, EcfParameter parameterToCheck, EcfErrorGroups errorGroup)
        {
            if (depenParams.FormatDef == null) { depenParams.FormatDef = parameterToCheck?.EcfFile?.Definition; }
            if (depenParams.ItemDef == null) { depenParams.ItemDef = parameterToCheck.Definition; }
            if (depenParams.Parent == null) { depenParams.Parent = parameterToCheck.Parent as EcfBlock; }
            if (depenParams.IsRoot == null) { depenParams.IsRoot = depenParams.Parent?.IsRoot() ?? true; }
            if (depenParams.Reference == null) { depenParams.Reference = parameterToCheck; }
            return CheckParameterInterFileDependencies(depenParams, filesToCheck, parameterToCheck.ValueGroups.ToList(), errorGroup);
        }
        public static List<EcfError> CheckParameterInterFileDependencies(EcfDependencyParameters depenParams,
            List<EgsEcfFile> filesToCheck, List<EcfValueGroup> groupsToCheck, EcfErrorGroups errorGroup)
        {
            List<EcfError> errors = new List<EcfError>();

            if ((depenParams.FormatDef?.IsDefiningItems ?? false) && string.Equals(depenParams.ItemDef.Name, depenParams.ParamKey_TemplateRoot))
            {
                errors.AddRange(CheckTemplateNames(filesToCheck, groupsToCheck, errorGroup));
            }
            if ((depenParams.FormatDef?.IsDefiningTemplates ?? false) && 
                !(depenParams.Parent != null ? depenParams.Parent.IsRoot() : depenParams.IsRoot.GetValueOrDefault(true)))
            {
                if (GetBlockListByBlockName(filesToCheck.Where(file => file.Definition.IsDefiningItems).ToList(), depenParams.ItemDef.Name).Count < 1)
                {
                    errors.Add(new EcfError(errorGroup, EcfErrors.IngredientItemNotFound, depenParams.ItemDef.Name));
                }
            }
            if ((depenParams.FormatDef?.IsDefiningBuildBlockGroups ?? false) && string.Equals(depenParams.ItemDef.Name, depenParams.ParamKey_Blocks))
            {
                errors.AddRange(CheckBuildBlockNames(filesToCheck, groupsToCheck, errorGroup));
            }

            return errors;
        }
        public static List<EcfError> CheckTemplateNames(List<EgsEcfFile> filesToCheck, List<EcfValueGroup> templateNamesToCheck, EcfErrorGroups errorGroup)
        {
            return templateNamesToCheck.SelectMany(group => group.Values)
                .Where(value => GetBlockListByBlockName(filesToCheck.Where(file => file.Definition.IsDefiningTemplates).ToList(), value).Count < 1)
                .Select(value => new EcfError(errorGroup, EcfErrors.TemplateNotFound, value)).ToList();
        }
        public static List<EcfError> CheckBuildBlockNames(List<EgsEcfFile> filesToCheck, List<EcfValueGroup> buildBlockNamesToCheck, EcfErrorGroups errorGroup)
        {
            return buildBlockNamesToCheck.SelectMany(group => group.Values)
                .Where(value => GetBlockListByBlockName(filesToCheck.Where(file => file.Definition.IsDefiningBuildBlocks).ToList(), value).Count < 1)
                .Select(value => new EcfError(errorGroup, EcfErrors.BuildBlockNotFound, value)).ToList();
        }

        private static IKeyItemComparer KeyItemComparer { get; } = new IKeyItemComparer();
        private class IKeyItemComparer : IEqualityComparer<EcfKeyValueItem>
        {
            public bool Equals(EcfKeyValueItem item1, EcfKeyValueItem item2)
            {
                if (item1 is null && item2 is null) return true;
                if (item1 is null || item2 is null) return false;
                return item1.Key.Equals(item2.Key);
            }
            public int GetHashCode(EcfKeyValueItem item)
            {
                return item.Key.GetHashCode();
            }
        }
    } 
    public static class EcfStructureTools
    {
        public static bool ValueListEquals(ReadOnlyCollection<string> ValueListA, ReadOnlyCollection<string> ValueListB)
        {
            if (ValueListA == null && ValueListB == null) { return true; }
            if (ValueListA == null || ValueListB == null) { return false; }
            if (ValueListA.Count != ValueListB.Count) { return false; }
            if (ValueListA.Count == 0) { return true; }
            for (int index = 0; index < ValueListA.Count; index++)
            {
                if (!string.Equals(ValueListA[index], ValueListB[index]))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool ValueGroupEquals(EcfValueGroup ValueGroupA, EcfValueGroup ValueGroupB)
        {
            if (ValueGroupA == null && ValueGroupB == null) { return true; }
            if (ValueGroupA == null || ValueGroupB == null) { return false; }
            return ValueListEquals(ValueGroupA.Values, ValueGroupB.Values);
        }
        public static bool ValueGroupListEquals(ReadOnlyCollection<EcfValueGroup> GroupListA, ReadOnlyCollection<EcfValueGroup> GroupListB)
        {
            if (GroupListA == null && GroupListB == null) { return true; }
            if (GroupListA == null || GroupListB == null) { return false; }
            if (GroupListA.Count != GroupListB.Count) { return false; }
            if (GroupListA.Count == 0) { return true; }
            for (int index = 0; index < GroupListA.Count; index++)
            {
                if (!ValueGroupEquals(GroupListA[index], GroupListB[index]))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool AttributeListEquals(ReadOnlyCollection<EcfAttribute> attributeListA, ReadOnlyCollection<EcfAttribute> attributeListB)
        {
            if (attributeListA == null && attributeListB == null) { return true; }
            if (attributeListA == null || attributeListB == null) { return false; }
            if (attributeListA.Count != attributeListB.Count) { return false; }
            if (attributeListA.Count == 0) { return true; }
            for (int index = 0; index < attributeListA.Count; index++)
            {
                EcfAttribute attrA = attributeListA[index];
                EcfAttribute attrB = attributeListB[index];
                if (!(attrA.IdEquals(attrB) && attrA.ContentEquals(attrB)))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool StructureItemIdEquals(EcfStructureItem itemA, EcfStructureItem itemB)
        {
            if (itemA == null && itemB == null) { return true; }
            if (itemA == null || itemB == null) { return false; }
            return itemA.IdEquals(itemB) && (!(itemA is EcfComment) || itemA.ContentEquals(itemB));
        }
        public static bool StructureItemListEquals(ReadOnlyCollection<EcfStructureItem> itemListA, ReadOnlyCollection<EcfStructureItem> itemListB)
        {
            if (itemListA == null && itemListB == null) { return true; }
            if (itemListA == null || itemListB == null) { return false; }
            if (itemListA.Count != itemListB.Count) { return false; }
            if (itemListA.Count == 0) { return true; }
            for (int index = 0; index < itemListA.Count; index++)
            {
                EcfStructureItem itemA = itemListA[index];
                EcfStructureItem itemB = itemListB[index];
                if (!(itemA.IdEquals(itemB) && itemA.ContentEquals(itemB)))
                {
                    return false;
                }
            }
            return true;
        }
        
        public static EcfStructureItem CopyStructureItem(EcfStructureItem template)
        {
            if (template is EcfComment comment)
            {
                return new EcfComment(comment);
            }
            else if (template is EcfAttribute attr)
            {
                return new EcfAttribute(attr);
            }
            else if (template is EcfParameter param)
            {
                return new EcfParameter(param);
            }
            else if (template is EcfBlock block)
            {
                return new EcfBlock(block);
            }
            return null;
        }

        public static List<EcfBlock> GetBlockListByBlockName(List<EgsEcfFile> files, EcfBlock block)
        {
            return GetBlockListByBlockName(files, block.GetName());
        }
        public static List<EcfBlock> GetBlockListByBlockName(List<EgsEcfFile> files, string blockName)
        {
            return files.SelectMany(file => file.GetDeepItemList<EcfBlock>()
                .Where(block => string.Equals(block.GetName(), blockName))).ToList();
        }
        public static List<EcfBlock> GetBlockListByParameterKey(List<EgsEcfFile> files, bool listRootBlocksOnly, EcfParameter parameter)
        {
            return GetBlockListByParameterKey(files, listRootBlocksOnly, parameter.Key);
        }
        public static List<EcfBlock> GetBlockListByParameterKey(List<EgsEcfFile> files, bool listRootBlocksOnly, params string[] parameterKeys)
        {
            return files.SelectMany(file => (listRootBlocksOnly ? file.GetItemList<EcfBlock>() : file.GetDeepItemList<EcfBlock>())
                .Where(block => parameterKeys?.Any(key => block.HasParameter(key, false, listRootBlocksOnly, out _)) ?? false)).ToList();
        }
        public static List<EcfBlock> GetBlockListByParameterValue(List<EgsEcfFile> files,
            bool withBlockNameCheck, bool withInheritedParams, string parameterValue, params string[] parameterKeys)
        {
            return files.SelectMany(file => file.GetDeepItemList<EcfBlock>())
                .Where(block => (withBlockNameCheck && string.Equals(block.GetName(), parameterValue)) ||
                    (parameterKeys?.Any(parameterName => block.HasParameter(parameterName, withInheritedParams, false, out EcfParameter parameter) && 
                        parameter.ContainsValue(parameterValue)) ?? false)
                ).ToList();
        }
        public static List<EcfBlock> GetBlockListByNameOrParamValue(List<EgsEcfFile> files,
            bool withBlockNameCheck, bool withInheritedParams, bool withSubParams, EcfBlock sourceBlock, params string[] parameterKeysInSource)
        {
            string blockName = sourceBlock.GetName();
            List<string> parameterValues = parameterKeysInSource?
                .SelectMany(key => sourceBlock.HasParameter(key, withInheritedParams, withSubParams, out EcfParameter parameter) ? 
                parameter.GetAllValues().ToList() : new List<string>()).ToList() ?? new List<string>();
            return files.SelectMany(file => file.GetDeepItemList<EcfBlock>()
                .Where(listedBlock =>
                {
                    string listedBlockName = listedBlock.GetName();
                    return (withBlockNameCheck && string.Equals(blockName, listedBlockName)) || 
                        parameterValues.Any(parameterValue => string.Equals(parameterValue, listedBlockName));
                }
                )).ToList();
        }

        public static EcfBaseItem FindRootItem(EcfBaseItem item)
        {
            if (item?.IsRoot() ?? true)
            {
                return item;
            }
            else
            {
                return FindRootItem(item?.Parent);
            }
        }
        public static int FindStructureLevel(EcfBaseItem item, int level)
        {
            if (item?.IsRoot() ?? true)
            {
                return level;
            }
            else
            {
                level++;
                return FindStructureLevel(item?.Parent, level);
            }
        }
        public static List<EcfDependency> FindBlockReferences(List<EcfBlock> completeBlockList, List<EcfBlock> blocksToCheck)
        {
            List<EcfDependency> dependencies = new List<EcfDependency>();
            blocksToCheck.ForEach(block =>
            {
                List<EcfBlock> inheritors = completeBlockList.Where(listedBlock => block.Equals(listedBlock.Inheritor)).ToList();
                inheritors.ForEach(inheritor =>
                {
                    dependencies.Add(new EcfDependency(EcfDependencies.IsInheritedBy, block, inheritor));
                });
            });
            return dependencies;
        }
        public static List<EcfDependency> FindAttributeInterFileDependencies(EcfDependencyParameters depenParams,
            List<EgsEcfFile> filesToCheck, List<EcfBlock> blocksToCheck)
        {
            return blocksToCheck.SelectMany(block => FindAttributeInterFileDependencies(depenParams, filesToCheck, block)).ToList();
        }
        public static List<EcfDependency> FindAttributeInterFileDependencies(EcfDependencyParameters depenParams, 
            List<EgsEcfFile> filesToCheck, EcfBlock blockToCheck)
        {
            if (depenParams.FormatDef == null) { depenParams.FormatDef = blockToCheck?.EcfFile?.Definition; }
            if (depenParams.Parent == null) { depenParams.Parent = blockToCheck.Parent as EcfBlock; }
            if (depenParams.IsRoot == null) { depenParams.IsRoot = depenParams.Parent?.IsRoot() ?? true; }
            if (depenParams.Reference == null) { depenParams.Reference = blockToCheck; }
            return FindAttributeInterFileDependencies(depenParams, filesToCheck, blockToCheck.GetName());
        }
        public static List<EcfDependency> FindAttributeInterFileDependencies(EcfDependencyParameters depenParams, List<EgsEcfFile> filesToCheck, string itemName)
        {
            List<EcfDependency> dependencies = new List<EcfDependency>();

            if (depenParams.FormatDef?.IsDefiningItems ?? false)
            {
                List<EcfBlock> templateList = GetBlockListByParameterKey(filesToCheck.Where(file => file.Definition.IsDefiningTemplates).ToList(), true, itemName);
                dependencies.AddRange(templateList.Select(template => new EcfDependency(EcfDependencies.IsUsedWith, depenParams.Reference as EcfBlock, template)));
            }
            if (depenParams.FormatDef?.IsDefiningTemplates ?? false)
            {
                List<EcfBlock> userList = GetBlockListByParameterValue(filesToCheck.Where(file => file.Definition.IsDefiningItems).ToList(),
                    true, true, itemName, depenParams.ParamKey_TemplateRoot);
                dependencies.AddRange(userList.Select(user => new EcfDependency(EcfDependencies.IsUsedWith, depenParams.Reference as EcfBlock, user)));
            }
            if (depenParams.FormatDef?.IsDefiningBuildBlocks ?? false)
            {
                List<EcfBlock> blockGroupList = GetBlockListByParameterValue(filesToCheck.Where(file => file.Definition.IsDefiningBuildBlockGroups).ToList(),
                    false, false, itemName, depenParams.ParamKey_Blocks);
                dependencies.AddRange(blockGroupList.Select(blockGroup => new EcfDependency(EcfDependencies.IsUsedWith, depenParams.Reference as EcfBlock, blockGroup)));
            }
            return dependencies;
        }
    }

    public class EgsEcfFile
    {
        public string FilePath { get; private set; } = null;
        public string FileName { get; private set; } = null;
        public int LineCount { get; private set; } = 0;
        public bool LoadAbortPending { get; set; } = false;
        public bool HasUnsavedData { get; private set; } = true;

        public Encoding FileEncoding { get; private set; } = null;
        public EcfFileNewLineSymbols NewLineSymbol { get; private set; } = EcfFileNewLineSymbols.Unknown;
        public FormatDefinition Definition { get; private set; } = null;

        public ReadOnlyCollection<EcfStructureItem> ItemList { get; }

        private List<EcfStructureItem> InternalItemList { get; } = new List<EcfStructureItem>();
        private StringBuilder EcfLineBuilder { get; } = new StringBuilder();
        private List<EcfError> StructuralErrors { get; } = new List<EcfError>();

        public EgsEcfFile(string filePathAndName, FormatDefinition definition, Encoding encoding, EcfFileNewLineSymbols newLineSymbol)
        {
            ItemList = InternalItemList.AsReadOnly();
            FileName = Path.GetFileName(filePathAndName);
            FilePath = Path.GetDirectoryName(filePathAndName);
            Definition = new FormatDefinition(definition);
            FileEncoding = encoding;
            NewLineSymbol = newLineSymbol;
            LineCount = File.Exists(filePathAndName) ? File.ReadLines(filePathAndName).Count() : 0;
        }
        public EgsEcfFile(string filePathAndName, FormatDefinition definition) : this(filePathAndName, definition, GetFileEncoding(filePathAndName), GetNewLineSymbol(filePathAndName))
        {
            
        }

        // file handling
        public void Load()
        {
            Load(null);
        }
        public void Load(IProgress<int> lineProgress)
        {
            string filePathAndName = Path.Combine(FilePath, FileName);
            try
            {
                using (StreamReader reader = new StreamReader(File.Open(filePathAndName, FileMode.Open, FileAccess.Read), FileEncoding))
                {
                    ParseEcfContent(lineProgress, reader);
                }
            }
            catch (Exception ex)
            {
                throw new IOException(string.Format("File {0} content could not be loaded: {1}", filePathAndName, ex.Message));
            }
        }
        public void Save()
        {
            Save(Path.Combine(FilePath, FileName), true, true, false);
        }
        public void Save(bool onlyValid, bool inheritError, bool allowFallback)
        {
            Save(Path.Combine(FilePath, FileName), onlyValid, inheritError, allowFallback);
        }
        public void Save(string filePathAndName, bool onlyValid, bool inheritError, bool allowFallback)
        {
            try
            {
                string path = Path.GetDirectoryName(filePathAndName);
                Directory.CreateDirectory(path);
                using (StreamWriter writer = new StreamWriter(File.Open(filePathAndName, FileMode.Create, FileAccess.Write), FileEncoding))
                {
                    writer.NewLine = GetNewLineChar(NewLineSymbol);
                    CreateEcfContent(writer, onlyValid, inheritError, allowFallback);
                }
                FileName = Path.GetFileName(filePathAndName);
                FilePath = path;
                LineCount = File.ReadLines(filePathAndName).Count();
                HasUnsavedData = false;
            }
            catch (Exception ex)
            {
                throw new IOException(string.Format("File {0} could not be saved: {1}", filePathAndName, ex.Message));
            }
        }
        public void SetUnsavedDataFlag()
        {
            if (!HasUnsavedData)
            {
                HasUnsavedData = true;
            }
        }
        public void ReplaceDefinition(FormatDefinition definition)
        {
            ReplaceDefinition(definition, null);
        }
        public void ReplaceDefinition(FormatDefinition definition, IProgress<int> lineProgress)
        {
            if (HasUnsavedData) { throw new InvalidOperationException("Definition replacement not allowed with unsaved data"); }
            Definition = new FormatDefinition(definition);
            Load(lineProgress);
        }

        // item handling
        public List<EcfError> GetErrorList()
        {
            List<EcfError> errors = new List<EcfError>(StructuralErrors);
            errors.AddRange(ItemList.Where(item => item is EcfStructureItem).Cast<EcfStructureItem>().SelectMany(item => item.GetDeepErrorList(true)));
            return errors;
        }
        public List<T> GetItemList<T>() where T : EcfStructureItem
        {
            return new List<T>(InternalItemList.Where(item => item is T).Cast<T>());
        }
        public List<T> GetDeepItemList<T>() where T : EcfStructureItem
        {
            return GetDeepItemList<T>(InternalItemList);
        }
        public static List<T> GetDeepItemList<T>(List<EcfStructureItem> itemList) where T : EcfStructureItem
        {
            List<T> list = new List<T>(itemList.Where(item => item is T).Cast<T>());
            list.AddRange(itemList.Where(item => item is EcfBlock).Cast<EcfBlock>().SelectMany(block => block.GetDeepChildList<T>()).ToList());
            return list;
        }
        public bool AddItem(EcfStructureItem item)
        {
            if (item != null)
            {
                item.UpdateStructureData(this, null);
                InternalItemList.Add(item);
                SetUnsavedDataFlag();
                return true;
            }
            return false;
        }
        public bool AddItem(EcfStructureItem item, int index)
        {
            if (item != null)
            {
                item.UpdateStructureData(this, null);
                if (index > InternalItemList.Count)
                {
                    InternalItemList.Add(item);
                }
                else
                {
                    if (index < 0) { index = 0; }
                    InternalItemList.Insert(index, item);
                }
                SetUnsavedDataFlag();
                return true;
            }
            return false;
        }
        public bool AddItem(EcfStructureItem item, EcfStructureItem precedingItem)
        {
            int index = InternalItemList.IndexOf(precedingItem);
            if (index < 0)
            {
                return AddItem(item);
            }
            else
            {
                return AddItem(item, index + 1);
            }
        }
        public int AddItem(List<EcfStructureItem> items)
        {
            int count = 0;
            items?.ForEach(item =>
            {
                if (AddItem(item))
                {
                    count++;
                }
            });
            return count;
        }
        public int AddItem(List<EcfStructureItem> items, int index)
        {
            int count = 0;
            items?.ForEach(item =>
            {
                if (AddItem(item, index))
                {
                    count++;
                    index++;
                }
            });
            return count;
        }
        public int AddItem(List<EcfStructureItem> items, EcfStructureItem precedingItem)
        {
            int index = InternalItemList.IndexOf(precedingItem);
            if (index < 0)
            {
                return AddItem(items);
            }
            else
            {
                return AddItem(items, index + 1);
            }
        }
        public bool RemoveItem(EcfStructureItem item)
        {
            if (item != null)
            {
                InternalItemList.Remove(item);
                SetUnsavedDataFlag();
                return true;
            }
            return false;
        }
        public int RemoveItem(List<EcfStructureItem> items)
        {
            int count = 0;
            items?.ForEach(item =>
            {
                if (RemoveItem(item))
                {
                    count++;
                }
            });
            return count;
        }
        public int Revalidate()
        {
            return InternalItemList.Sum(item => item.Revalidate());
        }

        // ecf creating
        private void CreateEcfContent(StreamWriter writer, bool onlyValid, bool inheritError, bool allowFallback)
        {
            int indent = 0;
            foreach (EcfStructureItem item in ItemList)
            {
                item.RemoveErrorsDeep(EcfErrors.BlockNotCreateable, EcfErrors.ParameterNotCreateable);
                if (item is EcfComment comment)
                {
                    CreateCommentLine(writer, comment, indent);
                }
                else if (item is EcfBlock rootBlock)
                {
                    CreateBlock(writer, rootBlock, indent, onlyValid, inheritError, allowFallback);
                }
            }
            StructuralErrors.Clear();
        }
        private void CreateCommentLine(StreamWriter writer, EcfComment comment, int indent)
        {
            CreateIndentedLineStart(indent);
            EcfLineBuilder.Append(Definition.WritingSingleLineCommentStart);
            EcfLineBuilder.Append(Definition.MagicSpacer);
            EcfLineBuilder.Append(string.Join(" / ", comment.Comments));
            writer.WriteLine(EcfLineBuilder.ToString());
        }
        private void CreateBlock(StreamWriter writer, EcfBlock block, int indent, bool onlyValid, bool inheritError, bool allowFallback)
        {
            if (!onlyValid || block.GetDeepErrorList(inheritError).Count == 0)
            {
                CreateBlockStartLine(writer, block, indent);
                CreateBlockContent(writer, block, indent, onlyValid, inheritError, allowFallback);
                CreateBlockEndLine(writer, indent);
            } 
            else if (allowFallback && block.IsParsingRawDataUseable())
            {
                CreateParsingDataLine(writer, block.OpenerLineParsingData);
                CreateBlockContent(writer, block, indent, onlyValid, inheritError, allowFallback);
                CreateParsingDataLine(writer, block.CloserLineParsingData);
            }
            else
            {
                block.AddError(new EcfError(EcfErrorGroups.Creating, EcfErrors.BlockNotCreateable, block.BuildRootId()));
            }
        }
        private void CreateBlockStartLine(StreamWriter writer, EcfBlock block, int indent)
        {
            CreateIndentedLineStart(indent);
            AppendBlockType(block);
            AppendAttributes(block.Attributes);
            AppendComments(block.Comments);
            writer.WriteLine(EcfLineBuilder.ToString());
        }
        private void CreateBlockContent(StreamWriter writer, EcfBlock block, int indent, bool onlyValid, bool inheritError, bool allowFallback)
        {
            indent++;
            foreach (EcfStructureItem item in block.ChildItems)
            {
                if (item is EcfComment comment)
                {
                    CreateCommentLine(writer, comment, indent);
                }
                else if (item is EcfBlock childBlock)
                {
                    CreateBlock(writer, childBlock, indent, onlyValid, inheritError, allowFallback);
                }
                else if (item is EcfParameter parameter)
                {
                    CreateParameterLine(writer, parameter, indent, onlyValid, inheritError, allowFallback);
                }
            }
        }
        private void CreateBlockEndLine(StreamWriter writer, int indent)
        {
            CreateIndentedLineStart(indent);
            EcfLineBuilder.Append(Definition.WritingBlockIdentifierPair.Closer);
            writer.WriteLine(EcfLineBuilder.ToString());
        }
        private void CreateParameterLine(StreamWriter writer, EcfParameter parameter, int indent, bool onlyValid, bool inheritError, bool allowFallback)
        {
            if (onlyValid && parameter.GetDeepErrorList(inheritError).Count == 0)
            {
                CreateIndentedLineStart(indent);
                AppendParameter(parameter);
                AppendAttributes(parameter.Attributes);
                AppendComments(parameter.Comments);
                writer.WriteLine(EcfLineBuilder.ToString());
            }
            else if (allowFallback && parameter.LineParsingDataUseable())
            {
                CreateParsingDataLine(writer, parameter.LineParsingData);
            }
            else
            {
                parameter.AddError(new EcfError(EcfErrorGroups.Creating, EcfErrors.ParameterNotCreateable, parameter.Key));
            }
        }
        private void CreateIndentedLineStart(int indent)
        {
            EcfLineBuilder.Clear();
            while (indent > 0)
            {
                EcfLineBuilder.Append(Definition.MagicSpacer + Definition.MagicSpacer);
                indent--;
            }
        }
        private void CreateParsingDataLine(StreamWriter writer, string parsingData)
        {
            EcfLineBuilder.Clear();
            EcfLineBuilder.Append(parsingData);
            writer.WriteLine(EcfLineBuilder.ToString());
        }
        private void AppendComments(ReadOnlyCollection<string> comments)
        {
            if (comments.Count > 0)
            {
                EcfLineBuilder.Append(Definition.MagicSpacer);
                EcfLineBuilder.Append(Definition.WritingSingleLineCommentStart);
                EcfLineBuilder.Append(Definition.MagicSpacer);
                EcfLineBuilder.Append(string.Join(" / ", comments));
            }
        }
        private void AppendBlockType(EcfBlock block)
        {
            EcfLineBuilder.Append(Definition.WritingBlockIdentifierPair.Opener);
            EcfLineBuilder.Append(Definition.MagicSpacer);
            EcfLineBuilder.Append(block.PreMark ?? string.Empty);
            EcfLineBuilder.Append(block.DataType ?? string.Empty);
            EcfLineBuilder.Append(block.PostMark ?? string.Empty);
        }
        private void AppendAttributes(ReadOnlyCollection<EcfAttribute> attributes)
        {
            List<string> attributeItems = attributes.Select(attr => CreateItem(attr)).ToList();
            EcfLineBuilder.Append(string.Join((Definition.ItemSeperator + Definition.MagicSpacer), attributeItems));
        }
        private void AppendParameter(EcfParameter parameter)
        {
            EcfLineBuilder.Append(CreateItem(parameter));
            if (parameter.Attributes.Count > 0)
            {
                EcfLineBuilder.Append(Definition.ItemSeperator);
                EcfLineBuilder.Append(Definition.MagicSpacer);
            }
        }
        private string CreateItem(EcfKeyValueItem keyValueItem)
        {
            StringBuilder item = new StringBuilder(keyValueItem.Key);
            if (keyValueItem.ValueGroups.Any(group => group.Values.Count > 0))
            {
                item.Append(Definition.ItemValueSeperator);
                item.Append(Definition.MagicSpacer);
                bool escaped = false;

                if ((keyValueItem.Definition?.IsForceEscaped ?? false) || keyValueItem.IsUsingGroups() || keyValueItem.HasMultiValue())
                {
                    item.Append(Definition.WritingEscapeIdentifiersPair.Opener);
                    escaped = true;
                }
                item.Append(string.Join(Definition.ValueGroupSeperator, keyValueItem.ValueGroups.Where(group => group.Values.Count > 0)
                    .Select(group => string.Join(Definition.ValueSeperator, group.Values))));
                if (escaped)
                {
                    item.Append(Definition.WritingEscapeIdentifiersPair.Closer);
                }
            }
            return item.ToString();
        }

        // ecf parsing
        private void ParseEcfContent(IProgress<int> lineProgress, StreamReader reader)
        {
            List<EcfStructureItem> rootItems = new List<EcfStructureItem>();
            List<EcfError> fatalErrors = new List<EcfError>();  

            List<StackItem> stack = new List<StackItem>();
            string rawLineData;
            string processedlineData;
            int lineCount = 0;
            int level = 0;
            List<string> comments = new List<string>();
            bool parameterLine;
            StringPairDefinition inCommentBlockPair = null;

            // parse content
            LoadAbortPending = false;
            while (!reader.EndOfStream)
            {
                // interprete next line
                if (LoadAbortPending) { break; }
                lineCount++;
                lineProgress?.Report(lineCount);
                rawLineData = reader.ReadLine();
                processedlineData = TrimOuterPhrases(Definition, rawLineData);
                if (!processedlineData.Equals(string.Empty))
                {
                    // comments
                    comments.Clear();
                    comments.AddRange(ParseComments(Definition, processedlineData, out processedlineData, inCommentBlockPair, out inCommentBlockPair));
                    if (processedlineData.Equals(string.Empty))
                    {
                        EcfComment comment = new EcfComment(comments);
                        if (level > 0)
                        {
                            stack[level - 1].Block.AddChild(comment);
                        }
                        else
                        {
                            comment.UpdateStructureData(this, null);
                            rootItems.Add(comment);
                        }
                        continue;
                    }
                    // Block opener
                    StringPairDefinition blockIdPair = Definition.BlockIdentifierPairs.FirstOrDefault(pair => processedlineData.StartsWith(pair.Opener));
                    if (blockIdPair != null)
                    {
                        EcfBlock block = ParseBlockElement(Definition, level < 1, processedlineData, lineCount);
                        // comments
                        block.AddComment(comments);
                        // raw data fallback
                        block.OpenerLineParsingData = rawLineData;
                        level++;
                        if (stack.Count < level)
                        {
                            stack.Add(new StackItem(block, lineCount, processedlineData, blockIdPair));
                        }
                        else
                        {
                            stack[level - 1] = new StackItem(block, lineCount, processedlineData, blockIdPair);
                        }
                    }
                    // parameter or block closer
                    else if (level > 0)
                    {
                        StackItem stackItem = stack[level - 1];
                        parameterLine = false;
                        // parameter
                        if (!stackItem.BlockSymbolPair.Closer.Equals(processedlineData))
                        {
                            parameterLine = true;
                            EcfBlock block = stackItem.Block;
                            try
                            {
                                EcfParameter parameter = ParseParameter(Definition, processedlineData, block, lineCount);
                                // comments
                                parameter.AddComment(comments);
                                // raw data fallback
                                parameter.LineParsingData = rawLineData;
                            }
                            catch (EcfException ex)
                            {
                                fatalErrors.Add(new EcfError(EcfErrorGroups.Structural, ex.EcfError, string.Format("{0} / {1}", block.GetFullPath(), ex.TextData), lineCount));
                            }
                        }
                        // block closer
                        if (processedlineData.EndsWith(stackItem.BlockSymbolPair.Closer))
                        {
                            level--;
                            EcfBlock block = stackItem.Block;
                            // completeness
                            List<EcfError> errors = CheckParametersValid(block.ChildItems.Where(child => child is EcfParameter)
                                .Cast<EcfParameter>().ToList(), Definition.BlockParameters, EcfErrorGroups.Interpretation);
                            errors.ForEach(error => { if (error != null) { error.LineInFile = stackItem.LineNumber; } });
                            block.AddError(errors);
                            // comments
                            if (!parameterLine) { block.AddComment(comments); }
                            // raw data fallback
                            block.CloserLineParsingData = rawLineData;
                            // append block to parent
                            if (level > 0)
                            {
                                StackItem parent = stack[level - 1];
                                parent.Block.AddChild(block);
                            }
                            // append block to root list
                            else
                            {
                                block.UpdateStructureData(this, null);
                                rootItems.Add(block);
                            }
                        }
                    }
                    // reporting unassigned line or unopend block
                    else
                    {
                        if (Definition.BlockIdentifierPairs.Any(pair => processedlineData.StartsWith(pair.Closer) || processedlineData.EndsWith(pair.Closer)))
                        {
                            fatalErrors.Add(new EcfError(EcfErrorGroups.Structural, EcfErrors.BlockCloserWithoutOpener, processedlineData, lineCount));
                        }
                        else
                        {
                            fatalErrors.Add(new EcfError(EcfErrorGroups.Structural, EcfErrors.ParameterWithoutParent, processedlineData, lineCount));
                        }

                    }
                }
            }
            if (!LoadAbortPending)
            {
                // reporting unclosed blocks 
                while (level > 0)
                {
                    StackItem item = stack[level - 1];
                    fatalErrors.Add(new EcfError(EcfErrorGroups.Structural, EcfErrors.BlockOpenerWithoutCloser, 
                        string.Format("{0} / {1}", item.Block.GetFullPath(), item.LineData), item.LineNumber));
                    level--;
                }

                // global error checks
                List<EcfBlock> completeBlockList = GetDeepItemList<EcfBlock>(rootItems);
                List<EcfError> errors = new List<EcfError>();
                completeBlockList.ForEach(block =>
                {
                    errors.Clear();
                    errors.AddRange(CheckBlockUniqueness(block, completeBlockList, EcfErrorGroups.Interpretation));
                    errors.Add(CheckBlockReferenceValid(block, completeBlockList, out EcfBlock inheriter, EcfErrorGroups.Interpretation));
                    block.Inheritor = inheriter;
                    block.AddError(errors);
                });

                // update der daten
                InternalItemList.Clear();
                AddItem(rootItems);
                StructuralErrors.Clear();
                StructuralErrors.AddRange(fatalErrors);
            }
        }
        private static List<string> ParseComments(FormatDefinition definition, string inLineData, out string outLineData, 
            StringPairDefinition inCommentBlockPair, out StringPairDefinition outCommentBlockPair)
        {
            List<string> comments = new List<string>();
            comments.AddRange(ParseBlockComment(definition, inLineData, out inLineData, inCommentBlockPair, out inCommentBlockPair));
            comments.AddRange(ParseSingleLineComment(definition, inLineData, out inLineData));
            comments.AddRange(ParseInLineComment(definition, inLineData, out inLineData));
            comments.AddRange(ParseMultiLineComment(definition, inLineData, out inLineData, inCommentBlockPair, out inCommentBlockPair));
            outLineData = inLineData;
            outCommentBlockPair = inCommentBlockPair;
            return comments;
        }
        private static List<string> ParseBlockComment(FormatDefinition definition, string inLineData, out string outLineData, 
            StringPairDefinition inCommentBlockPair, out StringPairDefinition outCommentBlockPair)
        {
            List<string> comments = new List<string>();
            if (inCommentBlockPair != null)
            {
                int end = inLineData.IndexOf(inCommentBlockPair.Closer);
                if (end >= 0)
                {
                    comments.Add(TrimComment(definition, inLineData.Substring(0, end)));
                    inLineData = inLineData.Remove(0, end + inCommentBlockPair.Closer.Length).Trim();
                    inCommentBlockPair = null;
                }
                else
                {
                    comments.Add(TrimComment(definition, inLineData));
                    inLineData = "";
                }
            }
            outLineData = inLineData;
            outCommentBlockPair = inCommentBlockPair;
            return comments;
        }
        private static List<string> ParseSingleLineComment(FormatDefinition definition, string inLineData, out string outLineData)
        {
            List<string> comments = new List<string>();
            string singleLineMark = definition.SingleLineCommentStarts.Where(mark => inLineData.IndexOf(mark) >= 0).OrderBy(mark => inLineData.IndexOf(mark)).FirstOrDefault();
            if (singleLineMark != null)
            {
                int start = inLineData.IndexOf(singleLineMark);
                comments.Add(TrimComment(definition, inLineData.Substring(start).Remove(0, singleLineMark.Length)));
                inLineData = inLineData.Remove(start).Trim();
            }
            outLineData = inLineData;
            return comments;
        }
        private static List<string> ParseInLineComment(FormatDefinition definition, string inLineData, out string outLineData)
        {
            List<string> comments = new List<string>();
            StringPairDefinition inLineMark = null;
            do
            {
                inLineMark = definition.MultiLineCommentPairs.FirstOrDefault(pair => {
                    int start = inLineData.IndexOf(pair.Opener);
                    if (start >= 0)
                    {
                        int end = inLineData.IndexOf(pair.Closer, start);
                        if (end >= 0)
                        {
                            comments.Add(TrimComment(definition, inLineData.Substring(start + pair.Opener.Length, end - start - pair.Opener.Length)));
                            inLineData = inLineData.Remove(start, end - start + pair.Closer.Length).Trim();
                            return true;
                        }
                    }
                    return false;
                });
            } while (inLineMark != null);
            outLineData = inLineData;
            return comments;
        }
        private static List<string> ParseMultiLineComment(FormatDefinition definition, string inLineData, out string outLineData, 
            StringPairDefinition inCommentBlockPair, out StringPairDefinition outCommentBlockPair)
        {
            List<string> comments = new List<string>();
            StringPairDefinition multiLineMark;
            multiLineMark = definition.MultiLineCommentPairs.Where(pair => inLineData.IndexOf(pair.Closer) >= 0)
                .OrderByDescending(pair => inLineData.IndexOf(pair.Closer)).LastOrDefault();
            if (multiLineMark != null)
            {
                int end = inLineData.IndexOf(multiLineMark.Closer);
                if (end >= 0)
                {
                    comments.Add(TrimComment(definition, inLineData.Substring(0, end)));
                    inLineData = inLineData.Remove(0, end + multiLineMark.Closer.Length).Trim();
                    inCommentBlockPair = null;
                }
            }
            multiLineMark = definition.MultiLineCommentPairs.Where(pair => inLineData.IndexOf(pair.Opener) >= 0)
                .OrderByDescending(pair => inLineData.IndexOf(pair.Opener)).FirstOrDefault();
            if (multiLineMark != null)
            {
                int start = inLineData.IndexOf(multiLineMark.Opener);
                if (start >= 0)
                {
                    comments.Add(TrimComment(definition, inLineData.Substring(start + multiLineMark.Opener.Length)));
                    inLineData = inLineData.Remove(start).Trim();
                    inCommentBlockPair = multiLineMark;
                }
            }
            outLineData = inLineData;
            outCommentBlockPair = inCommentBlockPair;
            return comments;
        }
        private static EcfBlock ParseBlockElement(FormatDefinition definition, bool isRoot, string lineData, int lineInFile)
        {
            ReadOnlyCollection<ItemDefinition> attributeDefinitions = isRoot ? definition.RootBlockAttributes : definition.ChildBlockAttributes;
            ReadOnlyCollection<BlockValueDefinition> dataTypeDefinitions = isRoot ? definition.RootBlockTypes : definition.ChildBlockTypes;

            lineData = TrimPairs(lineData, definition.BlockIdentifierPairs);
            string preMark = ParseBlockPreMark(definition, lineData);
            string blockType = ParseBlockType(definition, lineData, preMark, out string postMark);
            lineData = RemoveBlockType(lineData, preMark, blockType, postMark);
            Queue<string> splittedItems = SplitEcfItems(definition, lineData);
            EcfBlock block = new EcfBlock(preMark, blockType, postMark);
            List<EcfAttribute> attributes = ParseAttributes(definition, splittedItems, attributeDefinitions, lineInFile);
            block.AddAttribute(attributes);
            List<EcfError> errors = new List<EcfError>();
            errors.AddRange(CheckBlockPreMark(preMark, definition.BlockTypePreMarks, EcfErrorGroups.Interpretation));
            errors.AddRange(CheckBlockDataType(blockType, dataTypeDefinitions, EcfErrorGroups.Interpretation));
            errors.AddRange(CheckBlockPostMark(postMark, definition.BlockTypePostMarks, EcfErrorGroups.Interpretation));
            errors.AddRange(CheckAttributesValid(attributes, attributeDefinitions, EcfErrorGroups.Interpretation));
            errors.ForEach(error => { if (error != null) { error.LineInFile = lineInFile; } });
            block.AddError(errors);
            return block;
        }
        private static EcfParameter ParseParameter(FormatDefinition definition, string lineData, EcfBlock block, int lineInFile)
        {
            lineData = TrimPairs(lineData, definition.BlockIdentifierPairs);
            Queue<string> splittedItems = SplitEcfItems(definition, lineData);
            EcfParameter parameter = ParseParameterBase(definition, splittedItems, block, lineInFile);
            List<EcfAttribute> attributes = ParseAttributes(definition, splittedItems, definition.ParameterAttributes, lineInFile);
            parameter.AddAttribute(attributes);
            List<EcfError> errors = CheckAttributesValid(attributes, definition.ParameterAttributes, EcfErrorGroups.Interpretation);
            errors.ForEach(error => { if (error != null) { error.LineInFile = lineInFile; } });
            parameter.AddError(errors);
            return parameter;
        }
        private static string ParseBlockPreMark(FormatDefinition definition, string lineData)
        {
            return definition.BlockTypePreMarks.Where(mark => !string.IsNullOrEmpty(mark.Value)).FirstOrDefault(mark => lineData.StartsWith(mark.Value))?.Value;
        }
        private static string ParseBlockType(FormatDefinition definition, string lineData, string preMark, out string postMark)
        {
            postMark = null;
            if (preMark != null) { lineData = lineData.Remove(0, preMark.Length); }
            StringBuilder blockTypeItem = new StringBuilder();
            StringPairDefinition escapePair = null;
            string buffer;
            for (int index = 0; index < lineData.Length; index++)
            {
                buffer = lineData[index].ToString();
                if (escapePair == null)
                {
                    escapePair = definition.EscapeIdentifiersPairs.FirstOrDefault(pair => pair.Opener.Equals(buffer));
                    if (escapePair == null && definition.BlockTypePostMarks.Where(mark => !string.IsNullOrEmpty(mark.Value)).Any(mark => mark.Value.Equals(buffer)))
                    {
                        postMark = buffer;
                        break;
                    }
                }
                else if (escapePair.Closer.Equals(buffer))
                {
                    escapePair = null;
                }
                blockTypeItem.Append(buffer);
            }
            return blockTypeItem.Length > 0 ? blockTypeItem.ToString() : null;
        }
        private static  List<EcfAttribute> ParseAttributes(FormatDefinition definition, Queue<string> splittedItems, ReadOnlyCollection<ItemDefinition> definedAttributes, int lineInFile)
        {
            List<EcfError> errors = new List<EcfError>();
            List<EcfAttribute> attributes = new List<EcfAttribute>();
            while (splittedItems.Count > 0)
            {
                string key = splittedItems.Dequeue();
                errors.Clear();
                List<EcfValueGroup> groups = null;
                errors.Add(CheckItemUnknown(definedAttributes, key, KeyValueItemTypes.Attribute, out ItemDefinition itemDefinition, EcfErrorGroups.Interpretation));
                if (itemDefinition != null && itemDefinition.HasValue)
                {
                    if (splittedItems.Count > 0)
                    {
                        groups = ParseValues(definition, splittedItems.Dequeue());
                    }
                    errors.AddRange(CheckValuesValid(groups, itemDefinition, definition, EcfErrorGroups.Interpretation));
                }
                EcfAttribute attr = new EcfAttribute(key);
                attributes.Add(attr);
                attr.AddValue(groups);
                errors.ForEach(error => { if (error != null) { error.LineInFile = lineInFile; } });
                attr.AddError(errors);
            }
            return attributes;
        }
        private static EcfParameter ParseParameterBase(FormatDefinition definition, Queue<string> splittedItems, EcfBlock block, int lineInFile)
        {
            string key = null;
            List<EcfValueGroup> groups = null;
            List<EcfError> errors = new List<EcfError>();
            ItemDefinition itemDefinition = null;
            if (splittedItems.Count > 0)
            {
                key = splittedItems.Dequeue();
                errors.Add(CheckItemUnknown(definition.BlockParameters, key, KeyValueItemTypes.Parameter, out itemDefinition, EcfErrorGroups.Interpretation));
                if (itemDefinition != null && itemDefinition.HasValue)
                {
                    if (splittedItems.Count > 0)
                    {
                        groups = ParseValues(definition, splittedItems.Dequeue());
                    }
                    errors.AddRange(CheckValuesValid(groups, itemDefinition, definition, EcfErrorGroups.Interpretation));
                }
            }
            EcfParameter parameter = new EcfParameter(key, groups, null);
            block.AddChild(parameter);
            errors.ForEach(error => { if (error != null) { error.LineInFile = lineInFile; } });
            parameter.AddError(errors);
            return parameter;
        }
        private static List<EcfValueGroup> ParseValues(FormatDefinition definition, string itemValue)
        {
            List<EcfValueGroup> groups = new List<EcfValueGroup>();
            string[] valueGroups = TrimPairs(itemValue, definition.EscapeIdentifiersPairs)?.Split(definition.ValueGroupSeperator.ToArray());
            if (valueGroups != null)
            {
                foreach (string groupValues in valueGroups)
                {
                    groups.Add(new EcfValueGroup(groupValues.Trim().Split(definition.ValueSeperator.ToArray()).Select(value => value.Trim()).ToList()));
                }
            }
            return groups;
        }
        private static string TrimStarts(string lineData, ReadOnlyCollection<string> definedStarts)
        {
            string startMark = null;
            if (lineData != null)
            {
                do
                {
                    startMark = definedStarts.FirstOrDefault(start => lineData.StartsWith(start));
                    if (startMark != null)
                    {
                        lineData = lineData.Remove(0, startMark.Length).Trim();
                    }
                } while (startMark != null);
            }
            return lineData?.Trim();
        }
        private static string TrimPairs(string lineData, ReadOnlyCollection<StringPairDefinition> definedIdentifiers)
        {
            StringPairDefinition blockPair = null;
            if (lineData != null)
            {
                do
                {
                    blockPair = definedIdentifiers.FirstOrDefault(pair => lineData.StartsWith(pair.Opener));
                    if (blockPair != null)
                    {
                        lineData = lineData.Remove(0, blockPair.Opener.Length).Trim();
                    }
                } while (blockPair != null);
                do
                {
                    blockPair = definedIdentifiers.FirstOrDefault(pair => lineData.EndsWith(pair.Closer));
                    if (blockPair != null)
                    {
                        lineData = lineData.Remove(lineData.Length - blockPair.Closer.Length, blockPair.Closer.Length).Trim();
                    }
                } while (blockPair != null);
            }
            return lineData?.Trim();
        }
        private static string TrimComment(FormatDefinition definition, string comment)
        {
            if (comment != null)
            {
                comment = TrimStarts(comment.Trim(), definition.SingleLineCommentStarts).Trim();
                comment = TrimPairs(comment, definition.MultiLineCommentPairs).Trim();
            }
            return comment;
        }
        private static string TrimOuterPhrases(FormatDefinition definition, string lineData)
        {
            string foundPhrase;
            if (lineData != null)
            {
                do
                {
                    foundPhrase = definition.OuterTrimmingPhrases.FirstOrDefault(phrase => lineData.StartsWith(phrase));
                    if (foundPhrase != null)
                    {
                        lineData = lineData.Remove(0, foundPhrase.Length).Trim();
                    }
                } while (foundPhrase != null);
                do
                {
                    foundPhrase = definition.OuterTrimmingPhrases.FirstOrDefault(phrase => lineData.EndsWith(phrase));
                    if (foundPhrase != null)
                    {
                        lineData = lineData.Remove(lineData.Length - foundPhrase.Length, foundPhrase.Length).Trim();
                    }
                } while (foundPhrase != null);
            }
            return lineData?.Trim();
        }
        private static string RemoveBlockType(string lineData, string preMark, string blockType, string postMark)
        {
            return lineData.Remove(0, (preMark?.Length ?? 0) + (blockType?.Length ?? 0) + (postMark?.Length ?? 0)).Trim();
        }
        private static Queue<string> SplitEcfItems(FormatDefinition definition, string lineData)
        {
            Queue<string> splittedData = new Queue<string>();
            StringBuilder dataPart = new StringBuilder();
            StringPairDefinition escapePair = null;
            string buffer;
            bool split = false;

            // walk char by char
            for (int index = 0; index < lineData.Length; index++)
            {
                buffer = lineData[index].ToString();
                split = false;
                if (escapePair == null)
                {
                    escapePair = definition.EscapeIdentifiersPairs.FirstOrDefault(pair => pair.Opener.Equals(buffer));
                    if (escapePair == null)
                    {
                        split = ((buffer == definition.ItemSeperator) || (buffer == definition.ItemValueSeperator));
                    }
                }
                else if (escapePair.Closer.Equals(buffer))
                {
                    escapePair = null;
                }
                if (split)
                {
                    splittedData.Enqueue(dataPart.ToString().Trim());
                    dataPart.Clear();
                }
                else
                {
                    dataPart.Append(buffer);
                }
            }
            if (dataPart.Length > 0 || split)
            {
                splittedData.Enqueue(dataPart.ToString().Trim());
            }
            return splittedData;
        }

        private class StackItem
        {
            public EcfBlock Block { get; }
            public int LineNumber { get; }
            public string LineData { get; }
            public StringPairDefinition BlockSymbolPair { get; }
            public StackItem(EcfBlock block, int lineNumber, string lineData, StringPairDefinition blockSymbolPair)
            {
                Block = block;
                LineNumber = lineNumber;
                LineData = lineData;
                BlockSymbolPair = blockSymbolPair;
            }
        }
    }

    // Ecf Error Handling
    public enum EcfErrorGroups
    {
        Structural,
        Interpretation,
        Editing,
        Creating,
    }
    public enum EcfErrors
    {
        Unknown,

        // Exceptional
        KeyNullOrEmpty,

        // fatals
        BlockOpenerWithoutCloser,
        BlockCloserWithoutOpener,
        BlockNotCreateable,

        ParameterWithoutParent,
        ParameterNotCreateable,

        ValueGroupIndexInvalid,

        ValueIndexInvalid,

        // manageables
        BlockIdNotUnique,
        BlockInheritorMissing,
        BlockDataTypeMissing,
        BlockDataTypeUnknown,
        BlockPreMarkMissing,
        BlockPreMarkUnknown,
        BlockPostMarkMissing,
        BlockPostMarkUnknown,

        ParameterUnknown,
        ParameterMissing,
        ParameterDoubled,

        AttributeUnknown,
        AttributeMissing,
        AttributeDoubled,

        ValueGroupEmpty,
        
        ValueNull,
        ValueEmpty,
        ValueContainsProhibitedPhrases,

        // inter file
        TemplateNotFound,
        IngredientItemNotFound,
        BuildBlockNotFound,
    }
    public class EcfError
    {
        public EcfErrors Type { get; }
        public string Info { get; }
        public EcfErrorGroups Group { get; private set; }
        public int LineInFile { get; set; } = 0;
        public EcfStructureItem SourceItem { get; set; } = null;

        public EcfError(EcfErrorGroups group, EcfErrors type, string info)
        {
            Type = type;
            Info = info;
            Group = group;
        }
        public EcfError(EcfErrorGroups group, EcfErrors type, string info, int lineInFile) : this(group, type, info)
        {
            LineInFile = lineInFile;
        }

        // copy constructor
        public EcfError(EcfError template)
        {
            Type = template.Type;
            Info = template.Info;
            Group = template.Group;
            LineInFile = template.LineInFile;
            SourceItem = template.SourceItem;
        }

        // public
        public bool IsFromParsing()
        {
            return Group == EcfErrorGroups.Structural || Group == EcfErrorGroups.Interpretation;
        }
        public override string ToString()
        {
            StringBuilder errorText = new StringBuilder();
            if (IsFromParsing())
            {
                errorText.Append("In Line ");
                errorText.Append(LineInFile.ToString());
                errorText.Append(" at '");
            }
            else
            {
                errorText.Append("At '");
            }
            errorText.Append(SourceItem?.GetFullPath() ?? "unknown");
            errorText.Append("' occured error ");
            errorText.Append(Type.ToString());
            errorText.Append(", additional info: '");
            errorText.Append(Info ?? "null");
            errorText.Append("'");
            return errorText.ToString();
        }
    }
    public class EcfException : Exception
    {
        public EcfErrors EcfError { get; }
        public string TextData { get; }


        public EcfException() : base(ToString(EcfErrors.Unknown, ""))
        {
            EcfError = EcfErrors.Unknown;
            TextData = "";
        }

        public EcfException(EcfErrors ecfError) : base(ToString(ecfError, ""))
        {
            EcfError = ecfError;
            TextData = "";
        }

        public EcfException(EcfErrors ecfError, string textData) : base(ToString(ecfError, textData))
        {
            EcfError = ecfError;
            TextData = textData ?? "null";
        }

        public EcfException(EcfErrors ecfError, string textData, Exception inner) : base(ToString(ecfError, textData), inner)
        {
            EcfError = ecfError;
            TextData = textData ?? "null";
        }

        public override string ToString()
        {
            return ToString(EcfError, TextData);
        }
        private static string ToString(EcfErrors ecfError, string textData)
        {
            return string.Format("{0} in: '{1}'", ecfError, textData);
        }
    }

    // ecf dependency handling
    public enum EcfDependencies
    {
        IsUsedWith,
        IsInheritedBy,
    }
    public class EcfDependency
    {
        public EcfDependencies Type { get; }
        public EcfBlock SourceItem { get; } = null;
        public EcfBlock TargetItem { get; } = null;

        public EcfDependency(EcfDependencies type, EcfBlock sourceItem, EcfBlock targetItem)
        {
            Type = type;
            SourceItem = sourceItem;
            TargetItem = targetItem;
        }

        // copy constructor
        public EcfDependency(EcfDependency template)
        {
            Type = template.Type;
            SourceItem = template.SourceItem;
            TargetItem = template.TargetItem;
        }

        // public
        public override string ToString()
        {
            return string.Format("'{0}' {1} '{2}'", 
                SourceItem?.GetFullPath() ?? "unknown", Type.ToString(), TargetItem?.GetFullPath() ?? "unknown");
        }
    }
    public class EcfDependencyParameters
    {
        public EcfStructureItem Reference { get; set; } = null;
        public bool? IsRoot { get; set; } = null;
        public EcfStructureItem Parent { get; set; } = null;

        public FormatDefinition FormatDef { get; set; } = null;
        public ItemDefinition ItemDef { get; set; } = null;

        public string ParamKey_TemplateRoot { get; set; } = null;
        public string ParamKey_Blocks { get; set; } = null;
    }

    // ecf data Structure Classes
    public abstract class EcfBaseItem
    {
        protected string DefaultName { get; }
        public EgsEcfFile EcfFile { get; private set; } = null;
        public EcfStructureItem Parent { get; private set; } = null;
        public int StructureLevel { get; private set; } = -1;

        public EcfBaseItem(string defaultName)
        {
            DefaultName = defaultName;
        }

        // copy constructor
        public EcfBaseItem(EcfBaseItem template)
        {
            DefaultName = template.DefaultName;
            EcfFile = template.EcfFile;
            Parent = template.Parent;
            StructureLevel = template.StructureLevel;
        }

        // publics
        public abstract override string ToString();
        public bool IsRoot()
        {
            return Parent == null;
        }
        public void UpdateStructureData(EgsEcfFile file, EcfStructureItem parent)
        {
            EcfFile = file;
            Parent = parent;
            StructureLevel = FindStructureLevel(this, 0);
            OnStructureDataUpdate();
        }

        // private
        protected abstract void OnStructureDataUpdate();
    }
    public abstract class EcfStructureItem : EcfBaseItem
    {
        private List<EcfError> InternalErrors { get; } = new List<EcfError>();
        public ReadOnlyCollection<EcfError> Errors { get; }
        private List<string> InternalComments { get; } = new List<string>();
        public ReadOnlyCollection<string> Comments { get; }

        public EcfStructureItem(string defaultName) : base(defaultName)
        {
            Errors = InternalErrors.AsReadOnly();
            Comments = InternalComments.AsReadOnly();
        }

        // copy constructor
        public EcfStructureItem(EcfStructureItem template) : base(template)
        {
            Errors = InternalErrors.AsReadOnly();
            Comments = InternalComments.AsReadOnly();

            AddError(template.Errors.Cast<EcfError>().Select(error => 
            {
                EcfError newError = new EcfError(error)
                {
                    SourceItem = this,
                };
                return newError;
            }).ToList());
            AddComment(template.InternalComments);
        }

        public abstract List<EcfError> GetDeepErrorList(bool includeStructure);
        public abstract int RemoveErrorsDeep(params EcfErrors[] errors);
        public abstract string GetFullPath();
        public abstract int Revalidate();
        public abstract bool IdEquals(EcfStructureItem other);
        public abstract bool ContentEquals(EcfStructureItem other);
        public abstract void UpdateContent(EcfStructureItem template);

        // publics
        public EcfStructureItem BuildDeepCopy()
        {
            switch (this)
            {
                case EcfComment comment: return new EcfComment(comment);
                case EcfAttribute attribute: return new EcfAttribute(attribute);
                case EcfParameter parameter: return new EcfParameter(parameter);
                case EcfBlock block: return new EcfBlock(block);
                default: throw new NotImplementedException(string.Format("DeepCopy not implemented for {0}", GetType().ToString()));
            }
        }
        public int GetIndexInStructureLevel()
        {
            if (IsRoot())
            {
                return EcfFile.ItemList.IndexOf(this);
            }
            else
            {
                return (Parent as EcfBlock)?.ChildItems.IndexOf(this) ?? -1;
            }
        }
        public int GetIndexInStructureLevel<T>() where T : EcfStructureItem
        {
            List<EcfStructureItem> items;
            if (IsRoot())
            {
                items = EcfFile.ItemList.Where(item => item is EcfStructureItem).Cast<EcfStructureItem>().ToList();
            }
            else
            {
                items = (Parent as EcfBlock)?.ChildItems.Where(item => item is EcfStructureItem).Cast<EcfStructureItem>().ToList();
            }
            return items?.Where(child => child is T).Cast<T>().ToList().IndexOf((T)this) ?? -1;
        }
        public bool AddError(EcfError error)
        {
            if (error != null)
            {
                error.SourceItem = this;
                InternalErrors.Add(error);
                return true;
            }
            return false;
        }
        public int AddError(List<EcfError> errors)
        {
            int count = 0;
            errors?.ForEach(error => {
                if (AddError(error))
                {
                    count++;
                }
            });
            return count;
        }
        public int RemoveErrors(params EcfErrors[] errors)
        {
            int count = 0;
            foreach (EcfErrors error in errors)
            {
                count += RemoveErrors(error);
            }
            return count;
        }
        public bool ContainsError(EcfErrors error)
        {
            return InternalErrors.Any(err => err.Type.Equals(error));
        }
        public bool AddComment(string text)
        {
            if (IsKeyValid(text))
            {
                InternalComments.Add(text);
                EcfFile?.SetUnsavedDataFlag();
                return true;
            }
            return false;
        }
        public int AddComment(List<string> comments)
        {
            int count = 0;
            comments?.ForEach(comment => {
                if (AddComment(comment))
                {
                    count++;
                }
            });
            return count;
        }
        public void ClearComments()
        {
            InternalComments.Clear();
        }

        // privates
        protected bool RemoveError(EcfError error)
        {
            if (error != null)
            {
                InternalErrors.Remove(error);
                return true;
            }
            return false;
        }
        protected int RemoveErrors(List<EcfError> errors)
        {
            int count = 0;
            errors?.ForEach(error => {
                if (RemoveError(error))
                {
                    count++;
                }
            });
            return count;
        }
        private int RemoveErrors(EcfErrors error)
        {
            return RemoveErrors(Errors.Where(err => err.Type.Equals(error)).ToList());
        }
    }
    public abstract class EcfKeyValueItem : EcfStructureItem
    {
        public string Key { get; private set; }
        public ReadOnlyCollection<ItemDefinition> DefinedKeyValueItems { get; private set; } = null;
        public ItemDefinition Definition { get; private set; } = null;

        private List<EcfValueGroup> InternalValueGroups { get; } = new List<EcfValueGroup>();
        public ReadOnlyCollection<EcfValueGroup> ValueGroups { get; }

        private KeyValueItemTypes? ItemType { get; set; } = null;

        public enum KeyValueItemTypes
        {
            Parameter,
            Attribute,
        }

        public EcfKeyValueItem(string key, KeyValueItemTypes itemType, string defaultName) : base(defaultName)
        {
            UpdateKey(key);
            ItemType = itemType;
            ValueGroups = InternalValueGroups.AsReadOnly();
        }

        // copy constructor
        public EcfKeyValueItem(EcfKeyValueItem template) : base (template)
        {
            Key = template.Key;
            Definition = template.Definition == null ? null : new ItemDefinition(template.Definition);
            DefinedKeyValueItems = template.DefinedKeyValueItems.Select(def => new ItemDefinition(def)).ToList().AsReadOnly();
            ValueGroups = InternalValueGroups.AsReadOnly();
            ItemType = template.ItemType;
            AddValue(template.ValueGroups.Select(group => new EcfValueGroup(group)).ToList());
        }

        // publics
        public void UpdateKey(string key)
        {
            CheckKey(key);
            Key = key;
        }
        public void UpdateDefinition(ReadOnlyCollection<ItemDefinition> definitionGroup)
        {
            DefinedKeyValueItems = definitionGroup;
            if (DefinedKeyValueItems == null) { Definition = null; return; }
            CheckItemUnknown(definitionGroup, Key, ItemType, out ItemDefinition itemDefinition, EcfErrorGroups.Editing);
            Definition = itemDefinition;
        }
        public bool IsUsingGroups()
        {
            return InternalValueGroups.Count(group => group.Values.Count > 0) > 1;
        }
        public bool HasValue()
        {
            return (InternalValueGroups.FirstOrDefault()?.Values.Count ?? 0) > 0;
        }
        public bool HasMultiValue()
        {
            return InternalValueGroups.Any(group => group.Values.Count > 1);
        }
        public bool AddValue(string value)
        {
            if (InternalValueGroups.Count == 0)
            {
                EcfValueGroup group = new EcfValueGroup();
                group.UpdateStructureData(EcfFile, this);
                InternalValueGroups.Add(group);
            }
            return AddValue(value, 0);
        }
        public bool AddValue(string value, int groupIndex)
        {
            if (groupIndex < 0 || groupIndex >= InternalValueGroups.Count) { throw new EcfException(EcfErrors.ValueGroupIndexInvalid, groupIndex.ToString()); }
            return InternalValueGroups[groupIndex].AddValue(value);
        }
        public int AddValue(List<string> values)
        {
            int count = 0;
            values?.ForEach(value => {
                if (AddValue(value))
                {
                    count++;
                }
            });
            return count;
        }
        public int AddValue(List<string> values, int groupIndex)
        {
            if (groupIndex < 0 || groupIndex >= InternalValueGroups.Count) { throw new EcfException(EcfErrors.ValueGroupIndexInvalid, groupIndex.ToString()); }
            return InternalValueGroups[groupIndex].AddValue(values);
        }
        public bool AddValue(EcfValueGroup valueGroup)
        {
            if (valueGroup != null) {
                valueGroup.UpdateStructureData(EcfFile, this);
                InternalValueGroups.Add(valueGroup);
                EcfFile?.SetUnsavedDataFlag();
                return true;
            }
            return false;
        }
        public int AddValue(List<EcfValueGroup> valueGroups)
        {
            int count = 0;
            valueGroups?.ForEach(group => {
                if (AddValue(group))
                {
                    count++;
                }
            });
            return count;
        }
        public ReadOnlyCollection<string> GetAllValues()
        {
            return InternalValueGroups.SelectMany(group => group.Values).ToList().AsReadOnly();
        }
        public string GetFirstValue()
        {
            try
            {
                return GetValue(0);
            }
            catch(Exception)
            {
                return null;
            }
        }
        public string GetValue(int valueIndex)
        {
            try { 
                return GetValue(valueIndex, 0);
            }
            catch(Exception)
            {
                return null;
            }
}
        public string GetValue(int valueIndex, int groupIndex)
        {
            if (groupIndex < 0 || groupIndex >= InternalValueGroups.Count) { throw new EcfException(EcfErrors.ValueGroupIndexInvalid, groupIndex.ToString()); }
            if (valueIndex < 0 || valueIndex >= InternalValueGroups[groupIndex].Values.Count) { throw new EcfException(EcfErrors.ValueIndexInvalid, valueIndex.ToString()); }
            return InternalValueGroups[groupIndex].Values[valueIndex];
        }
        public void ClearValues()
        {
            InternalValueGroups.Clear();
        }
        public int CountValues()
        {
            return InternalValueGroups.Sum(group => group.Values.Count);
        }
        public int IndexOf(EcfValueGroup group)
        {
            return InternalValueGroups.IndexOf(group);
        }
        public bool ContainsValue(string value)
        {
            return GetAllValues().Any(storedValue => storedValue.Equals(value));
        }
        public bool RemoveValue(string value)
        {
            foreach (EcfValueGroup group in InternalValueGroups)
            {
                if (group.RemoveValue(value))
                {
                    return true;
                }
            }
            return false;
        }
        public int RemoveValue(List<string> values)
        {
            return InternalValueGroups.Sum(group => group.RemoveValue(values));
        }
        public int RemoveAllValue(string value)
        {
            return InternalValueGroups.Sum(group => group.RemoveAllValue(value));
        }
        public int RemoveAllValue(List<string> values)
        {
            return InternalValueGroups.Sum(group => group.RemoveAllValue(values));
        }

        // privates
        private void CheckKey(string key)
        {
            if (!IsKeyValid(key)) { throw new EcfException(EcfErrors.KeyNullOrEmpty, GetType().Name); }
        }
        protected int RevalidateKeyValue()
        {
            if (DefinedKeyValueItems == null) { throw new InvalidOperationException("Validation is only possible with File reference"); }

            int errorCount = 0;
            errorCount += RevalidateKey() ? 1 : 0;
            if (Definition != null) { errorCount += RevalidateValues(); }
            return errorCount;
        }
        private bool RevalidateKey()
        {
            RemoveErrors(EcfErrors.ParameterUnknown, EcfErrors.AttributeUnknown);
            bool result = AddError(CheckItemUnknown(DefinedKeyValueItems, Key, ItemType, out ItemDefinition itemDefinition, EcfErrorGroups.Editing));
            Definition = itemDefinition;
            return result;
        }
        private int RevalidateValues()
        {
            RemoveErrors(EcfErrors.ValueGroupEmpty, EcfErrors.ValueNull, EcfErrors.ValueEmpty, EcfErrors.ValueContainsProhibitedPhrases);
            return AddError(CheckValuesValid(InternalValueGroups, Definition, EcfFile?.Definition, EcfErrorGroups.Editing));
        }
    }
    public class EcfAttribute : EcfKeyValueItem
    {
        public EcfAttribute(string key) : base(key, KeyValueItemTypes.Attribute, "Attribute")
        {
            
        }
        public EcfAttribute(string key, string value) : this(key)
        {
            AddValue(value);
        }
        public EcfAttribute(string key, List<string> values) : this(key)
        {
            AddValue(values);
        }
        public EcfAttribute(string key, List<EcfValueGroup> valueGroups) : this(key)
        {
            AddValue(valueGroups);
        }

        // copyconstructor
        public EcfAttribute(EcfAttribute template) : base(template)
        {

        }

        // public
        public override int Revalidate()
        {
            return RevalidateKeyValue();
        }
        public override string ToString()
        {
            return string.Format("{0} {1}, values: {2}, errors: {3}", 
                DefaultName, Key, ValueGroups.Sum(group => group.Values.Count).ToString(), Errors.Count.ToString());
        }
        public override List<EcfError> GetDeepErrorList(bool includeStructure)
        {
            return Errors.ToList();
        }
        public override bool IdEquals(EcfStructureItem other)
        {
            if (!(other is EcfAttribute otherAttribute)) { return false; }
            return Key.Equals(otherAttribute.Key);
        }
        public override bool ContentEquals(EcfStructureItem other)
        {
            if (!(other is EcfAttribute otherAttribute)) { return false; }
            return ValueGroupListEquals(ValueGroups, otherAttribute.ValueGroups) && ValueListEquals(Comments, other.Comments);
        }
        public override void UpdateContent(EcfStructureItem template)
        {
            if (!(template is EcfAttribute attr)) { throw new InvalidOperationException("Structure item type not matching!"); }
            ClearComments();
            AddComment(attr.Comments.ToList());
            ClearValues();
            AddValue(attr.ValueGroups.Select(group => new EcfValueGroup(group)).ToList());
        }
        public override int RemoveErrorsDeep(params EcfErrors[] errors)
        {
            return RemoveErrors(errors);
        }
        public override string GetFullPath()
        {
            StringBuilder name = new StringBuilder();
            if (!IsRoot())
            {
                name.Append(Parent.GetFullPath());
                name.Append(" / ");
            }
            name.Append(Key);
            return name.ToString();
        }

        // privates
        protected override void OnStructureDataUpdate()
        {
            foreach (EcfValueGroup group in ValueGroups)
            {
                group.UpdateStructureData(EcfFile, this);
            }
        }
    }
    public class EcfParameter : EcfKeyValueItem
    {
        public string LineParsingData { get; set; } = null;

        private List<EcfAttribute> InternalAttributes { get; } = new List<EcfAttribute>();
        public ReadOnlyCollection<EcfAttribute> Attributes { get; }

        public EcfParameter(string key) : base(key, KeyValueItemTypes.Parameter, "Parameter")
        {
            Attributes = InternalAttributes.AsReadOnly();
        }
        public EcfParameter(string key, List<string> values, List<EcfAttribute> attributes) : this(key)
        {
            AddValue(values);
            AddAttribute(attributes);
        }
        public EcfParameter(string key, List<EcfValueGroup> valueGroups, List<EcfAttribute> attributes) : this(key)
        {
            AddValue(valueGroups);
            AddAttribute(attributes);
        }
        
        // copy constructor
        public EcfParameter(EcfParameter template) : base(template)
        {
            Attributes = InternalAttributes.AsReadOnly();

            LineParsingData = template.LineParsingData;

            AddAttribute(template.Attributes.Select(attribute => new EcfAttribute(attribute)).ToList());
        }

        // publics
        public override string ToString()
        {
            return string.Format("{0} {1}, values: {2}, attributes: {3}, errors: {4}",
                DefaultName, Key, ValueGroups.Sum(group => group.Values.Count).ToString(), Attributes.Count.ToString(), Errors.Count.ToString());
        }
        public override string GetFullPath()
        {
            StringBuilder name = new StringBuilder();
            if (!IsRoot())
            {
                name.Append(Parent.GetFullPath());
                name.Append(" / ");
            }
            name.Append(Key);
            return name.ToString();
        }
        public override List<EcfError> GetDeepErrorList(bool includeStructure)
        {
            List<EcfError> errors = new List<EcfError>(Errors);
            errors.AddRange(InternalAttributes.SelectMany(attribute => attribute.GetDeepErrorList(includeStructure)));
            return errors;
        }
        public override bool IdEquals(EcfStructureItem other)
        {
            if (!(other is EcfParameter otherParameter)) { return false; }
            return Key.Equals(otherParameter.Key);
        }
        public override bool ContentEquals(EcfStructureItem other)
        {
            if (!(other is EcfParameter otherParameter)) { return false; }
            return ValueGroupListEquals(ValueGroups, otherParameter.ValueGroups) && 
                AttributeListEquals(Attributes, otherParameter.Attributes) && 
                ValueListEquals(Comments, other.Comments);
        }
        public override void UpdateContent(EcfStructureItem template)
        {
            if (!(template is EcfParameter param)) { throw new InvalidOperationException("Structure item type not matching!"); }
            ClearComments();
            AddComment(param.Comments.ToList());
            ClearValues();
            AddValue(param.ValueGroups.Select(group => new EcfValueGroup(group)).ToList());
            ClearAttributes();
            AddAttribute(param.Attributes.Select(attr => new EcfAttribute(attr)).ToList());
        }
        public override int RemoveErrorsDeep(params EcfErrors[] errors)
        {
            int count = RemoveErrors(errors);
            count += InternalAttributes.Sum(attribute => attribute.RemoveErrors(errors));
            return count;
        }
        public override int Revalidate()
        {
            int errorCount = RevalidateKeyValue();
            errorCount += RevalidateAttributes();
            errorCount += InternalAttributes.Sum(attr => attr.Revalidate());
            return errorCount;
        }
        public int RevalidateAttributes()
        {
            FormatDefinition definition = EcfFile?.Definition;
            if (definition == null) { throw new InvalidOperationException("Validation is only possible with File reference"); }

            RemoveErrors(EcfErrors.AttributeMissing, EcfErrors.AttributeDoubled);
            return AddError(CheckAttributesValid(InternalAttributes, definition.ParameterAttributes, EcfErrorGroups.Editing));
        }
        public bool AddAttribute(EcfAttribute attribute)
        {
            if (attribute != null)
            {
                attribute.UpdateStructureData(EcfFile, this);
                InternalAttributes.Add(attribute);
                EcfFile?.SetUnsavedDataFlag();
                return true;
            }
            return false;
        }
        public int AddAttribute(List<EcfAttribute> attributes)
        {
            int count = 0;
            attributes?.ForEach(attribute =>
            {
                if (AddAttribute(attribute))
                {
                    count++;
                }
            });
            return count;
        }
        public void ClearAttributes()
        {
            InternalAttributes.Clear();
        }
        public bool LineParsingDataUseable()
        {
            return !string.IsNullOrEmpty(LineParsingData);
        }

        // privates
        protected override void OnStructureDataUpdate()
        {
            UpdateDefinition(EcfFile?.Definition.BlockParameters);
            InternalAttributes.ForEach(attribute => {
                attribute.UpdateStructureData(EcfFile, this);
                attribute.UpdateDefinition(EcfFile?.Definition.ParameterAttributes);
            });
            foreach (EcfValueGroup group in ValueGroups)
            {
                group.UpdateStructureData(EcfFile, this);
            }
        }
    }
    public class EcfBlock : EcfStructureItem
    {
        public string PreMark { get; private set; }
        public string DataType { get; private set; }
        public string PostMark { get; private set; }

        public EcfBlock Inheritor { get; set; } = null;

        public string OpenerLineParsingData { get; set; } = null;
        public string CloserLineParsingData { get; set; } = null;

        private List<EcfAttribute> InternalAttributes { get; } = new List<EcfAttribute>();
        public ReadOnlyCollection<EcfAttribute> Attributes { get; }
        private List<EcfStructureItem> InternalChildItems { get; } = new List<EcfStructureItem>();
        public ReadOnlyCollection<EcfStructureItem> ChildItems { get; }

        public EcfBlock(string preMark, string blockType, string postMark) : base("Block")
        {
            Attributes = InternalAttributes.AsReadOnly();
            ChildItems = InternalChildItems.AsReadOnly();

            PreMark = preMark;
            DataType = blockType;
            PostMark = postMark;
        }
        public EcfBlock(string preMark, string blockType, string postMark, List<EcfAttribute> attributes, List<EcfStructureItem> childItems)
            : this(preMark, blockType, postMark)
        {
            AddAttribute(attributes);
            AddChild(childItems);
        }
        public EcfBlock(string preMark, string blockType, string postMark, List<EcfAttribute> attributes, List<EcfParameter> parameters)
            : this(preMark, blockType, postMark)
        {
            AddAttribute(attributes);
            AddChild(parameters);
        }
        public EcfBlock(string preMark, string blockType, string postMark, List<EcfAttribute> attributes, List<EcfBlock> blocks)
            : this(preMark, blockType, postMark)
        {
            AddAttribute(attributes);
            AddChild(blocks);
        }

        // copyconstructor
        public EcfBlock(EcfBlock template) : base(template)
        {
            Attributes = InternalAttributes.AsReadOnly();
            ChildItems = InternalChildItems.AsReadOnly();

            PreMark = template.PreMark;
            DataType = template.DataType;
            PostMark = template.PostMark;

            Inheritor = template.Inheritor;

            OpenerLineParsingData = template.OpenerLineParsingData;
            CloserLineParsingData = template.CloserLineParsingData;

            AddAttribute(template.Attributes.Select(attribute => new EcfAttribute(attribute)).ToList());
            AddChild(template.ChildItems.Select(child => CopyStructureItem(child)).ToList());
        }

        // publics
        public string GetId()
        {
            return GetAttributeFirstValue(EcfFile?.Definition.BlockIdAttribute);
        }
        public string GetName()
        {
            return GetAttributeFirstValue(EcfFile?.Definition.BlockNameAttribute);
        }
        public string GetRefTarget()
        {
            return GetAttributeFirstValue(EcfFile?.Definition.BlockReferenceTargetAttribute);
        }
        public string GetRefSource()
        {
            return GetAttributeFirstValue(EcfFile?.Definition.BlockReferenceSourceAttribute);
        }
        public bool SetId(string value)
        {
            EcfAttribute attribute = FindOrCreateAttribute(EcfFile?.Definition.BlockIdAttribute);
            attribute?.ClearValues();
            attribute?.AddValue(value);
            return attribute != null;
        }
        public bool SetName(string value)
        {
            EcfAttribute attribute = FindOrCreateAttribute(EcfFile?.Definition.BlockNameAttribute);
            attribute?.ClearValues();
            attribute?.AddValue(value);
            return attribute != null;
        }
        public bool SetRefTarget(string value)
        {
            EcfAttribute attribute = FindOrCreateAttribute(EcfFile?.Definition.BlockReferenceTargetAttribute);
            attribute?.ClearValues();
            attribute?.AddValue(value);
            return attribute != null;
        }
        public bool SetRefSource(string value)
        {
            EcfAttribute attribute = FindOrCreateAttribute(EcfFile?.Definition.BlockReferenceSourceAttribute);
            attribute?.ClearValues();
            attribute?.AddValue(value);
            return attribute != null;
        }
        public void UpdateTypeData(string preMark, string blockType, string postMark)
        {
            PreMark = preMark;
            DataType = blockType;
            PostMark = postMark;
        }
        public bool IsInheritingParameter(string paramName, out EcfParameter parameter)
        {
            if (HasParameter(paramName, out parameter))
            {
                return true;
            }
            return Inheritor?.IsInheritingParameter(paramName, out parameter) ?? false;
        }
        public string BuildRootId()
        {
            StringBuilder identification = new StringBuilder(DataType ?? string.Empty);
            if (!IsRoot())
            {
                identification.Append(" ");
                identification.Append(GetIndexInStructureLevel<EcfBlock>());
            }
            foreach (EcfAttribute attr in Attributes)
            {
                identification.Append(", ");
                identification.Append(attr.Key);
                if (attr.HasValue())
                {
                    identification.Append(": ");
                    identification.Append(attr.GetFirstValue());
                }
            }
            return identification.ToString();
        }
        public bool IsParsingRawDataUseable()
        {
            return !string.IsNullOrEmpty(OpenerLineParsingData) && !string.IsNullOrEmpty(CloserLineParsingData);
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}, name: {2}, childs: {3}, attributes: {4}, errors: {5}",
                DefaultName, DataType, GetName(), ChildItems.Count.ToString(), Attributes.Count.ToString(), Errors.Count.ToString());
        }
        public override List<EcfError> GetDeepErrorList(bool includeStructure)
        {
            List<EcfError> errors = new List<EcfError>(Errors);
            errors.AddRange(Attributes.SelectMany(attribute => attribute.GetDeepErrorList(includeStructure)));
            if (includeStructure)
            {
                errors.AddRange(ChildItems.Where(item => item is EcfStructureItem).Cast<EcfStructureItem>().SelectMany(item => item.GetDeepErrorList(includeStructure)));
            }
            return errors;
        }
        public override bool IdEquals(EcfStructureItem other)
        {
            if (!(other is EcfBlock otherBlock)) { return false; }
            return string.Equals(PreMark, otherBlock.PreMark) && 
                string.Equals(DataType, otherBlock.DataType) && 
                string.Equals(PostMark, otherBlock.PostMark) &&
                AttributeListEquals(Attributes, otherBlock.Attributes);
        }
        public override bool ContentEquals(EcfStructureItem other)
        {
            if (!(other is EcfBlock otherBlock)) { return false; }
            return ValueListEquals(Comments, otherBlock.Comments);
        }
        public override void UpdateContent(EcfStructureItem template)
        {
            if (!(template is EcfBlock block)) { throw new InvalidOperationException("Structure item type not matching!"); }
            ClearComments();
            AddComment(block.Comments.ToList());
        }
        public override int RemoveErrorsDeep(params EcfErrors[] errors)
        {
            int count = RemoveErrors(errors);
            count += InternalAttributes.Sum(attribute => attribute.RemoveErrors(errors));
            count += InternalChildItems.Sum(child => child.RemoveErrors(errors));
            return count;
        }
        public override string GetFullPath()
        {
            StringBuilder name = new StringBuilder();
            EcfBlock item = this;
            while (item != null)
            {
                if (name.Length > 0)
                {
                    name.Insert(0, " / ");
                }
                name.Insert(0, item.BuildRootId());
                item = item.Parent as EcfBlock;
            }
            return name.ToString();
        }
        public override int Revalidate()
        {
            FormatDefinition definition = EcfFile?.Definition;
            if (definition == null) { throw new InvalidOperationException("Validation is only possible for added elements"); }

            int errorCount = RevalidateDataType();
            errorCount += RevalidateParameters();
            errorCount += InternalChildItems.Sum(item => item.Revalidate());
            errorCount += RevalidateAttributes();
            errorCount += InternalAttributes.Sum(attr => attr.Revalidate());
            return errorCount;
        }
        protected override void OnStructureDataUpdate()
        {
            InternalAttributes.ForEach(attribute => {
                attribute.UpdateStructureData(EcfFile, this);
                attribute.UpdateDefinition(IsRoot() ? EcfFile?.Definition.RootBlockAttributes : EcfFile?.Definition.ChildBlockAttributes);
            });
            InternalChildItems.ForEach(child => child.UpdateStructureData(EcfFile, this));
        }

        public int RevalidateDataType()
        {
            FormatDefinition definition = EcfFile?.Definition;
            if (definition == null) { throw new InvalidOperationException("Validation is only possible with File reference"); }

            int errorCount = 0;

            RemoveErrors(EcfErrors.BlockPreMarkMissing, EcfErrors.BlockPreMarkUnknown, 
                EcfErrors.BlockDataTypeMissing, EcfErrors.BlockDataTypeUnknown,
                EcfErrors.BlockPostMarkMissing, EcfErrors.BlockPostMarkUnknown);
            
            errorCount += AddError(CheckBlockPreMark(PreMark, definition.BlockTypePreMarks, EcfErrorGroups.Editing));
            errorCount += AddError(CheckBlockDataType(DataType, IsRoot() ? definition.RootBlockTypes : definition.ChildBlockTypes, EcfErrorGroups.Editing));
            errorCount += AddError(CheckBlockPostMark(PostMark, definition.BlockTypePostMarks, EcfErrorGroups.Editing));
            return errorCount;
        }
        public int RevalidateUniqueness(List<EcfBlock> blockList)
        {
            if (EcfFile?.Definition == null) { throw new InvalidOperationException("Validation is only possible with File reference"); }
            
            RemoveErrors(EcfErrors.BlockIdNotUnique);
            return AddError(CheckBlockUniqueness(this, blockList, EcfErrorGroups.Editing));
        }
        public bool RevalidateReferenceHighLevel(List<EcfBlock> blockList)
        {
            RemoveErrors(EcfErrors.BlockInheritorMissing);
            if (Inheritor != null && !blockList.Contains(Inheritor))
            {
                return AddError(new EcfError(EcfErrorGroups.Editing, EcfErrors.BlockInheritorMissing, GetRefSource()));
            }
            return true;
        }
        public bool RevalidateReferenceRepairing(List<EcfBlock> blockList)
        {
            if (EcfFile?.Definition == null) { throw new InvalidOperationException("Validation is only possible with File reference"); }

            RemoveErrors(EcfErrors.BlockInheritorMissing);

            bool result = AddError(CheckBlockReferenceValid(this, blockList, out EcfBlock inheriter, EcfErrorGroups.Editing));
            Inheritor = inheriter;
            return result;
        }
        public int RevalidateParameters()
        {
            FormatDefinition definition = EcfFile?.Definition;
            if (definition == null) { throw new InvalidOperationException("Validation is only possible with File reference"); }

            RemoveErrors(EcfErrors.ParameterMissing, EcfErrors.ParameterDoubled);
            List<EcfParameter> parameters = ChildItems.Where(item => item is EcfParameter).Cast<EcfParameter>().ToList();
            return AddError(CheckParametersValid(parameters, definition.BlockParameters, EcfErrorGroups.Editing));
        }
        public int RevalidateAttributes()
        {
            FormatDefinition definition = EcfFile?.Definition;
            if (definition == null) { throw new InvalidOperationException("Validation is only possible with File reference"); }

            RemoveErrors(EcfErrors.AttributeMissing, EcfErrors.AttributeDoubled);
            return AddError(CheckAttributesValid(InternalAttributes, IsRoot() ? definition.RootBlockAttributes : definition.ChildBlockAttributes, EcfErrorGroups.Editing));
        }
        
        public EcfBlock GetFirstChildBlock()
        {
            return InternalChildItems.Where(child => child is EcfBlock).Cast<EcfBlock>().FirstOrDefault();
        }
        public List<T> GetDeepChildList<T>() where T : EcfBaseItem
        {
            List<T> childs = new List<T>(ChildItems.Where(child => child is T).Cast<T>());
            foreach (EcfBlock subBlock in ChildItems.Where(item => item is EcfBlock).Cast<EcfBlock>())
            {
                childs.AddRange(subBlock.GetDeepChildList<T>());
            }
            return childs;
        }
        public bool AddChild(EcfStructureItem child)
        {
            if (child != null)
            {
                child.UpdateStructureData(EcfFile, this);
                InternalChildItems.Add(child);
                EcfFile?.SetUnsavedDataFlag();
                return true;
            }
            return false;
        }
        public bool AddChild(EcfStructureItem child, int index)
        {
            if (child != null)
            {
                child.UpdateStructureData(EcfFile, this);
                if (index > InternalChildItems.Count)
                {
                    InternalChildItems.Add(child);
                }
                else
                {
                    if (index < 0) { index = 0; }
                    InternalChildItems.Insert(index, child);
                }
                EcfFile?.SetUnsavedDataFlag();
                return true;
            }
            return false;
        }
        public bool AddChild(EcfStructureItem child, EcfStructureItem precedingChild)
        {
            int index = InternalChildItems.IndexOf(precedingChild);
            if (index < 0)
            {
                return AddChild(child);
            }
            else
            {
                return AddChild(child, index + 1);
            }
        }
        public int AddChild(List<EcfStructureItem> childs)
        {
            int count = 0;
            childs?.ForEach(child => { 
                if (AddChild(child))
                {
                    count++;
                } 
            });
            return count;
        }
        public int AddChild(List<EcfParameter> parameters)
        {
            int count = 0;
            parameters?.ForEach(parameter => {
                if (AddChild(parameter))
                {
                    count++;
                }
            });
            return count;
        }
        public int AddChild(List<EcfBlock> blocks)
        {
            int count = 0;
            blocks?.ForEach(block => {
                if (AddChild(block))
                {
                    count++;
                }
            });
            return count;
        }
        public int AddChild(List<EcfStructureItem> childs, int index)
        {
            int count = 0;
            childs?.ForEach(child =>
            {
                if (AddChild(child, index))
                {
                    count++;
                    index++;
                }
            });
            return count;
        }
        public int AddChild(List<EcfStructureItem> childs, EcfStructureItem precedingChild)
        {
            int index = InternalChildItems.IndexOf(precedingChild);
            if (index < 0)
            {
                return AddChild(childs);
            }
            else
            {
                return AddChild(childs, index + 1);
            }
        }
        public bool RemoveChild(EcfStructureItem childItem)
        {
            if (childItem != null)
            {
                InternalChildItems.Remove(childItem);
                EcfFile?.SetUnsavedDataFlag();
                return true;
            }
            return false;
        }
        public int RemoveChild(List<EcfStructureItem> childItems)
        {
            int count = 0;
            childItems?.ForEach(child => {
                if (RemoveChild(child))
                {
                    count++;
                }
            });
            return count;
        }
        public int RemoveChild(List<EcfParameter> parameters)
        {
            int count = 0;
            parameters?.ForEach(parameter => {
                if (RemoveChild(parameter))
                {
                    count++;
                }
            });
            return count;
        }
        public int RemoveChild(List<EcfBlock> blocks)
        {
            int count = 0;
            blocks?.ForEach(block => {
                if (RemoveChild(block))
                {
                    count++;
                }
            });
            return count;
        }

        public string GetAttributeFirstValue(string attrName)
        {
            return InternalAttributes.FirstOrDefault(attr => attr.Key.Equals(attrName))?.GetFirstValue();
        }
        public bool HasAttribute(string attrName, out EcfAttribute attribute)
        {
            attribute = InternalAttributes.FirstOrDefault(attr => attr.Key.Equals(attrName));
            return attribute != null;
        }
        public EcfAttribute FindOrCreateAttribute(string key)
        {
            if (!HasAttribute(key, out EcfAttribute attribute))
            {
                FormatDefinition definition = EcfFile?.Definition;
                if (definition == null) { throw new InvalidOperationException("Attribute creation is only possible with file reference"); }
                ReadOnlyCollection<ItemDefinition> attributeDefinition = IsRoot() ? definition.RootBlockAttributes : definition.ChildBlockAttributes;
                if (!attributeDefinition.Any(attr => attr.Name.Equals(key))) { throw new InvalidOperationException(string.Format("Attribute key '{0}' is not allowed", key)); }

                attribute = new EcfAttribute(key);
                AddChild(attribute);
            }
            return attribute;
        }
        public bool AddAttribute(EcfAttribute attribute)
        {
            if (attribute != null)
            {
                attribute.UpdateStructureData(EcfFile, this);
                InternalAttributes.Add(attribute);
                EcfFile?.SetUnsavedDataFlag();
                return true;
            }
            return false;
        }
        public int AddAttribute(List<EcfAttribute> attributes)
        {
            int count = 0;
            attributes?.ForEach(attribute => 
            { 
                if (AddAttribute(attribute))
                {
                    count++;
                }
            });
            return count;
        }
        public void ClearAttributes()
        {
            InternalAttributes.Clear();
        }

        public string GetParameterFirstValue(string paramName)
        {
            return GetParameterFirstValue(paramName, true, false);
        }
        public string GetParameterFirstValue(string paramName, bool withInheritance, bool withSubBlocks)
        {
            HasParameter(paramName, withInheritance, withSubBlocks, out EcfParameter parameter);
            return parameter?.GetFirstValue();
        }
        public bool HasParameter(string key, out EcfParameter parameter)
        {
            return HasParameter(key, false, false, out parameter);
        }
        public bool HasParameter(string key, bool withInheritance, bool withSubBlocks, out EcfParameter parameter)
        {
            parameter = InternalChildItems.Where(item => item is EcfParameter).Cast<EcfParameter>().FirstOrDefault(param => param.Key.Equals(key));
            if (parameter == null && withSubBlocks)
            {
                parameter = GetDeepChildList<EcfParameter>().FirstOrDefault(param => param.Key.Equals(key));
            }
            if (parameter == null && withInheritance && Inheritor != null)
            {
                Inheritor.HasParameter(key, true, false, out parameter);
            }
            return parameter != null;
        }
        public EcfParameter FindOrCreateParameter(string key)
        {
            if (!HasParameter(key, out EcfParameter parameter))
            {
                FormatDefinition definition = EcfFile?.Definition;
                if (definition == null) { throw new InvalidOperationException("Parameter creation is only possible with file reference"); }
                if (!definition.BlockParameters.Any(param => param.Name.Equals(key))) { 
                    throw new InvalidOperationException(string.Format("Parameter key '{0}' is not defined for '{1}'", key, definition.FileType)); }

                parameter = new EcfParameter(key);
                AddChild(parameter);
            }
            return parameter;
        }
        public bool RemoveParameter(string key)
        {
            if (HasParameter(key, out EcfParameter parameter))
            {
                return RemoveChild(parameter);
            }
            return false;
        }
        public int RemoveParameter(List<string> keys)
        {
            return keys.Where(key => RemoveParameter(key) == true).Count();
        }
        public int RemoveParameterDeep(string key)
        {
            int count = RemoveParameter(key) ? 1: 0;
            count += InternalChildItems.Where(child => child is EcfBlock).Cast<EcfBlock>().Sum(subBlock => subBlock.RemoveParameterDeep(key));
            return count;
        }
        public int RemoveParameterDeep(List<string> keys)
        {
            int count = RemoveParameter(keys);
            count += InternalChildItems.Where(child => child is EcfBlock).Cast<EcfBlock>().Sum(subBlock => subBlock.RemoveParameterDeep(keys));
            return count;
        }
        public void ClearParameters()
        {
            InternalChildItems.RemoveAll(child => child is EcfParameter);
        }
    }
    public class EcfComment : EcfStructureItem
    {
        public EcfComment(string comment) : base("Comment")
        {
            AddComment(comment);
        }
        public EcfComment(List<string> comments) : base("Comment")
        {
            AddComment(comments);
        }

        // copy constructor
        public EcfComment(EcfComment template) : base(template)
        {

        }

        // publics
        public override List<EcfError> GetDeepErrorList(bool includeStructure)
        {
            return Errors.ToList();
        }
        public override bool IdEquals(EcfStructureItem other)
        {
            return other is EcfComment;
        }
        public override bool ContentEquals(EcfStructureItem other)
        {
            if (!(other is EcfComment)) { return false; }
            return ValueListEquals(Comments, other.Comments);
        }
        public override void UpdateContent(EcfStructureItem template)
        {
            if (!(template is EcfComment comment)) { throw new InvalidOperationException("Structure item type not matching!"); }
            ClearComments();
            AddComment(comment.Comments.ToList());
        }
        public override int RemoveErrorsDeep(params EcfErrors[] errors)
        {
            return RemoveErrors(errors);
        }
        public override string GetFullPath()
        {
            StringBuilder name = new StringBuilder();
            if (!IsRoot())
            {
                name.Append(Parent.GetFullPath());
                name.Append(" / ");
            }
            name.Append(DefaultName);
            name.Append(GetIndexInStructureLevel<EcfComment>().ToString());
            return name.ToString();
        }
        public override string ToString()
        {
            return string.Format("{0} {1}, comments {2}, errors: {3}", 
                DefaultName, GetIndexInStructureLevel<EcfComment>().ToString(), Comments.Count.ToString(), Errors.Count.ToString());
        }
        public override int Revalidate()
        {
            return 0;
        }

        // privates
        protected override void OnStructureDataUpdate()
        {
            
        }
    }
    public class EcfValueGroup : EcfBaseItem
    {
        private List<string> InternalValues { get; } = new List<string>();
        public ReadOnlyCollection<string> Values { get; }

        public EcfValueGroup() : base("ValueGroup")
        {
            Values = InternalValues.AsReadOnly();
        }
        public EcfValueGroup(string value) : this()
        {
            AddValue(value);
        }
        public EcfValueGroup(List<string> values) : this()
        {
            AddValue(values);
        }

        // copy constructor
        public EcfValueGroup(EcfValueGroup template) : base(template)
        {
            Values = InternalValues.AsReadOnly();
            AddValue(template.InternalValues);
        }

        public override string ToString()
        {
            return string.Format("{0}, values {1}", DefaultName, string.Join(" / ", InternalValues));
        }
        public bool AddValue(string value)
        {
            if (value != null) {
                InternalValues.Add(value);
                EcfFile?.SetUnsavedDataFlag();
                return true;
            }
            return false;
        }
        public int AddValue(List<string> values)
        {
            int count = 0;
            values?.ForEach(value =>
            {
                if (AddValue(value))
                {
                    count++;
                }
            });
            return count;
        }
        public bool RemoveValue(string value)
        {
            if (InternalValues.Remove(value))
            {
                EcfFile?.SetUnsavedDataFlag();
                return true;
            }
            return false;
        }
        public int RemoveValue(List<string> values)
        {
            int count = 0;
            values?.ForEach(value =>
            {
                if (RemoveValue(value))
                {
                    count++;
                }
            });
            return count;
        }
        public int RemoveAllValue(string value)
        {
            int count = InternalValues.RemoveAll(val => string.Equals(val, value));
            if (count > 0)
            {
                EcfFile?.SetUnsavedDataFlag();
            }
            return count;
        }
        public int RemoveAllValue(List<string> values)
        {
            return values.Sum(value => RemoveAllValue(value));
        }

        protected override void OnStructureDataUpdate()
        {
            
        }
    }

    // definition data structures
    public class FormatDefinition
    {
        public string FilePathAndName { get; }
        public string GameMode { get; }
        public string FileType { get; }

        public string BlockIdAttribute { get; }
        public string BlockNameAttribute { get; }
        public string BlockReferenceSourceAttribute { get; }
        public string BlockReferenceTargetAttribute { get; }

        public bool IsDefiningItems { get; }
        public bool IsDefiningTemplates { get; }
        public bool IsDefiningBuildBlocks { get; }
        public bool IsDefiningBuildBlockGroups { get; }
        public bool IsDefiningGlobalMacros { get; }
        public bool IsDefiningGlobalMacroUsers { get; }

        public ReadOnlyCollection<string> SingleLineCommentStarts { get; }
        public ReadOnlyCollection<StringPairDefinition> MultiLineCommentPairs { get; }
        public ReadOnlyCollection<StringPairDefinition> BlockIdentifierPairs { get; }
        public ReadOnlyCollection<string> OuterTrimmingPhrases { get; }
        public ReadOnlyCollection<StringPairDefinition> EscapeIdentifiersPairs { get; }

        public string ItemSeperator { get; }
        public string ItemValueSeperator { get; }
        public string ValueSeperator { get; }
        public string ValueGroupSeperator { get; }
        public string ValueFractionalSeperator { get; }
        public string MagicSpacer { get; }

        public ReadOnlyCollection<string> ProhibitedValuePhrases { get; }

        public ReadOnlyCollection<BlockValueDefinition> BlockTypePreMarks { get; }
        public ReadOnlyCollection<BlockValueDefinition> BlockTypePostMarks { get; }
        public ReadOnlyCollection<BlockValueDefinition> RootBlockTypes { get; }
        public ReadOnlyCollection<ItemDefinition> RootBlockAttributes { get; }
        public ReadOnlyCollection<BlockValueDefinition> ChildBlockTypes { get; }
        public ReadOnlyCollection<ItemDefinition> ChildBlockAttributes { get; }
        public ReadOnlyCollection<ItemDefinition> BlockParameters { get; }
        public ReadOnlyCollection<ItemDefinition> ParameterAttributes { get; }

        public string WritingSingleLineCommentStart { get; }
        public StringPairDefinition WritingBlockIdentifierPair { get;}
        public StringPairDefinition WritingEscapeIdentifiersPair { get; }

        public FormatDefinition(string filePathAndName, string gameMode, string fileType,
            string blockIdAttribute, string blockNameAttribute, string blockReferenceSourceAttribute, string blockReferenceTargetAttribute,
            bool isDefiningItems, bool isDefiningTemplates, bool isDefiningBuildBlocks, bool isDefiningBuildBlockGroups, 
            bool isDefiningGlobalMacros, bool isDefiningGlobalMacroUsers,
            List<string> singleLineCommentStarts, List<StringPairDefinition> multiLineCommentPairs,
            List<StringPairDefinition> blockPairs, List<StringPairDefinition> escapeIdentifierPairs, List<string> outerTrimmingPhrases,
            string itemSeperator, string itemValueSeperator, string valueSeperator, 
            string valueGroupSeperator, string valueFractionalSeperator, string magicSpacer,
            List<BlockValueDefinition> blockTypePreMarks, List<BlockValueDefinition> blockTypePostMarks,
            List<BlockValueDefinition> rootBlockTypes, List<ItemDefinition> rootBlockAttributes,
            List<BlockValueDefinition> childBlockTypes, List<ItemDefinition> childBlockAttributes,
            List<ItemDefinition> blockParameters, List<ItemDefinition> parameterAttributes)
        {
            FilePathAndName = filePathAndName;
            GameMode = gameMode;
            FileType = fileType;

            BlockIdAttribute = blockIdAttribute;
            BlockNameAttribute = blockNameAttribute;
            BlockReferenceSourceAttribute = blockReferenceSourceAttribute;
            BlockReferenceTargetAttribute = blockReferenceTargetAttribute;
            
            IsDefiningItems = isDefiningItems;
            IsDefiningTemplates = isDefiningTemplates;
            IsDefiningBuildBlocks = isDefiningBuildBlocks;
            IsDefiningBuildBlockGroups = isDefiningBuildBlockGroups;
            IsDefiningGlobalMacros = isDefiningGlobalMacros;
            IsDefiningGlobalMacroUsers = isDefiningGlobalMacroUsers;

            SingleLineCommentStarts = singleLineCommentStarts.AsReadOnly();
            MultiLineCommentPairs = multiLineCommentPairs.AsReadOnly();
            BlockIdentifierPairs = blockPairs.AsReadOnly();
            OuterTrimmingPhrases = outerTrimmingPhrases.AsReadOnly();
            EscapeIdentifiersPairs = escapeIdentifierPairs.AsReadOnly();

            ItemSeperator = itemSeperator;
            ItemValueSeperator = itemValueSeperator;
            ValueSeperator = valueSeperator;
            ValueGroupSeperator = valueGroupSeperator;
            ValueFractionalSeperator = valueFractionalSeperator;
            MagicSpacer = magicSpacer;

            HashSet<string> prohibitedPhrases = new HashSet<string>();
            foreach (string start in SingleLineCommentStarts) { prohibitedPhrases.Add(start); }
            foreach (StringPairDefinition pair in MultiLineCommentPairs) { prohibitedPhrases.Add(pair.Opener); prohibitedPhrases.Add(pair.Closer); }
            foreach (StringPairDefinition pair in BlockIdentifierPairs) { prohibitedPhrases.Add(pair.Opener); prohibitedPhrases.Add(pair.Closer); }
            foreach (StringPairDefinition pair in EscapeIdentifiersPairs) { prohibitedPhrases.Add(pair.Opener); prohibitedPhrases.Add(pair.Closer); }
            prohibitedPhrases.Add(ValueSeperator);
            prohibitedPhrases.Add(ValueGroupSeperator);
            ProhibitedValuePhrases = prohibitedPhrases.ToList().AsReadOnly();

            BlockTypePreMarks = blockTypePreMarks.AsReadOnly();
            BlockTypePostMarks = blockTypePostMarks.AsReadOnly();
            RootBlockTypes = rootBlockTypes.AsReadOnly();
            RootBlockAttributes = rootBlockAttributes.AsReadOnly();
            ChildBlockTypes = childBlockTypes.AsReadOnly();
            ChildBlockAttributes = childBlockAttributes.AsReadOnly();
            BlockParameters = blockParameters.AsReadOnly();
            ParameterAttributes = parameterAttributes.AsReadOnly();

            WritingSingleLineCommentStart = singleLineCommentStarts.First();
            WritingBlockIdentifierPair = blockPairs.First();
            WritingEscapeIdentifiersPair = escapeIdentifierPairs.First();
        }
        public FormatDefinition(FormatDefinition template)
        {
            FilePathAndName = template.FilePathAndName;
            GameMode = template.GameMode;
            FileType = template.FileType;

            BlockIdAttribute = template.BlockIdAttribute;
            BlockNameAttribute = template.BlockNameAttribute;
            BlockReferenceSourceAttribute = template.BlockReferenceSourceAttribute;
            BlockReferenceTargetAttribute = template.BlockReferenceTargetAttribute;
            
            IsDefiningItems = template.IsDefiningItems;
            IsDefiningTemplates = template.IsDefiningTemplates;
            IsDefiningBuildBlocks = template.IsDefiningBuildBlocks;
            IsDefiningBuildBlockGroups = template.IsDefiningBuildBlockGroups;
            IsDefiningGlobalMacros = template.IsDefiningGlobalMacros;
            IsDefiningGlobalMacroUsers = template.IsDefiningGlobalMacroUsers;

            SingleLineCommentStarts = template.SingleLineCommentStarts.ToList().AsReadOnly();
            MultiLineCommentPairs = template.MultiLineCommentPairs.Select(pair => new StringPairDefinition(pair)).ToList().AsReadOnly();
            BlockIdentifierPairs = template.BlockIdentifierPairs.Select(pair => new StringPairDefinition(pair)).ToList().AsReadOnly();
            OuterTrimmingPhrases = template.OuterTrimmingPhrases.ToList().AsReadOnly();
            EscapeIdentifiersPairs = template.EscapeIdentifiersPairs.Select(pair => new StringPairDefinition(pair)).ToList().AsReadOnly();

            ItemSeperator = template.ItemSeperator;
            ItemValueSeperator = template.ItemValueSeperator;
            ValueSeperator = template.ValueSeperator;
            ValueGroupSeperator = template.ValueGroupSeperator;
            ValueFractionalSeperator = template.ValueFractionalSeperator;
            MagicSpacer = template.MagicSpacer;

            ProhibitedValuePhrases = template.ProhibitedValuePhrases.ToList().AsReadOnly();

            BlockTypePreMarks = template.BlockTypePreMarks.Select(mark => new BlockValueDefinition(mark)).ToList().AsReadOnly();
            BlockTypePostMarks = template.BlockTypePostMarks.Select(mark => new BlockValueDefinition(mark)).ToList().AsReadOnly();
            RootBlockTypes = template.RootBlockTypes.Select(type => new BlockValueDefinition(type)).ToList().AsReadOnly();
            RootBlockAttributes = template.RootBlockAttributes.Select(item => new ItemDefinition(item)).ToList().AsReadOnly();
            ChildBlockTypes = template.ChildBlockTypes.Select(type => new BlockValueDefinition(type)).ToList().AsReadOnly();
            ChildBlockAttributes = template.ChildBlockAttributes.Select(item => new ItemDefinition(item)).ToList().AsReadOnly();
            BlockParameters = template.BlockParameters.Select(item => new ItemDefinition(item)).ToList().AsReadOnly();
            ParameterAttributes = template.ParameterAttributes.Select(item => new ItemDefinition(item)).ToList().AsReadOnly();

            WritingSingleLineCommentStart = template.WritingSingleLineCommentStart;
            WritingBlockIdentifierPair = template.WritingBlockIdentifierPair;
            WritingEscapeIdentifiersPair = template.WritingEscapeIdentifiersPair;
        }
    }
    public class BlockValueDefinition
    {
        public string Value { get; }
        public bool IsOptional { get; }

        public BlockValueDefinition(string value, string isOptional, bool valueCheck)
        {
            if (valueCheck && !IsKeyValid(value)) { throw new ArgumentException(string.Format("'{0}' is not a valid 'value' parameter", value)); }
            if (!valueCheck && value == null) { throw new ArgumentException("Null is not a valid 'value' parameter"); }
            if (!bool.TryParse(isOptional, out bool optional)) { throw new ArgumentException(string.Format("'{0}' is not a valid 'isOptional' parameter", isOptional)); }
            Value = value;
            IsOptional = optional;
        }
        public BlockValueDefinition(string value, string isOptional) : this(value, isOptional, true)
        {
            
        }
        public BlockValueDefinition(BlockValueDefinition template)
        {
            Value = template.Value;
            IsOptional = template.IsOptional;
        }
    }
    public class ItemDefinition
    {
        public string Name { get; }
        public bool IsOptional { get; }
        public bool HasValue { get; }
        public bool IsAllowingBlank { get; }
        public bool IsForceEscaped { get; }
        public string Info { get; }

        public ItemDefinition(string name, bool isOptional, bool hasValue, bool isAllowingBlank, bool isForceEscaped, string info)
        {
            if (!IsKeyValid(name)) { throw new ArgumentException(string.Format("'{0}' is not a valid 'name' parameter", name)); }
            Name = name;
            IsOptional = isOptional;
            HasValue = hasValue;
            IsAllowingBlank = isAllowingBlank;
            IsForceEscaped = isForceEscaped;
            Info = info ?? "";
        }
        public ItemDefinition(string name, string isOptional, string hasValue, string isAllowingBlank, string isForceEscaped, string info)
        {
            if (!IsKeyValid(name)) { throw new ArgumentException(string.Format("'{0}' is not a valid 'name' parameter", name)); }
            if (!bool.TryParse(isOptional, out bool optional)) { throw new ArgumentException(string.Format("'{0}' is not a valid 'isOptional' parameter", isOptional)); }
            if (!bool.TryParse(hasValue, out bool value)) { throw new ArgumentException(string.Format("'{0}' is not a valid 'hasValue' parameter", hasValue)); }
            if (!bool.TryParse(isAllowingBlank, out bool blank)) { throw new ArgumentException(string.Format("'{0}' is not a valid 'canBlank' parameter", isAllowingBlank)); }
            if (!bool.TryParse(isForceEscaped, out bool forceEscaped)) { throw new ArgumentException(string.Format("'{0}' is not a valid 'forceEscape' parameter", isForceEscaped)); }
            Name = name;
            IsOptional = optional;
            HasValue = value;
            IsAllowingBlank = blank;
            IsForceEscaped = forceEscaped;
            Info = info ?? "";
        }
        public ItemDefinition(ItemDefinition template)
        {
            Name = template.Name;
            IsOptional = template.IsOptional;
            HasValue = template.HasValue;
            IsAllowingBlank = template.IsAllowingBlank;
            IsForceEscaped = template.IsForceEscaped;
            Info = template.Info;
        }

        public override string ToString()
        {
            return string.Format("ItemDefinition: {0}, info: {1}", Name, Info);
        }
    }
    public class StringPairDefinition
    {
        public string Opener { get; }
        public string Closer { get; }
        public StringPairDefinition(string opener, string closer)
        {
            if (!IsKeyValid(opener)) { throw new ArgumentException(string.Format("'{0}' is not a valid 'opener' parameter", opener)); }
            if (!IsKeyValid(closer)) { throw new ArgumentException(string.Format("'{0}' is not a valid 'closer' parameter", closer)); }
            Opener = opener;
            Closer = closer;
        }
        public StringPairDefinition(StringPairDefinition template)
        {
            Opener = template.Opener;
            Closer = template.Closer;
        }
    }
}