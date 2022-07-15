using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static EgsEcfParser.EcfFormatting;

namespace EgsEcfParser
{
    public static class EcfFormatting
    {
        private static List<FormatDefinition> Definitions { get; } = new List<FormatDefinition>();
        private static bool DefinitionsLoaded { get; set; } = false;

        /// <summary>
        /// <para><returns>Reloads <see cref="FormatDefinition" /> from xml</returns></para> 
        /// <para>Exceptions:<br/>
        /// <see cref="IOException" /><br/>
        /// </para>
        /// </summary>
        public static void ReloadDefinitions()
        {
            DefinitionsLoaded = false;
            XmlLoading.LoadDefinitions();
        }
        /// <summary>
        /// <para><returns>Returns a list of supported Egs Ecf file types defined by xml</returns></para> 
        /// <para>Exceptions:<br/>
        /// <see cref="IOException" /><br/>
        /// </para>
        /// </summary>
        public static ReadOnlyCollection<string> GetSupportedFileTypes()
        {
            XmlLoading.LoadDefinitions();
            return Definitions.Select(def => def.FileType).ToList().AsReadOnly();
        }
        /// <summary>
        /// <para><returns>Tries to find a suitable <see cref="FormatDefinition" /> for <paramref name="filePathAndName" /></returns></para> 
        /// <para>Exceptions:<br/>
        /// <see cref="IOException" /><br/>
        /// </para>
        /// </summary>
        public static bool TryGetDefinition(string filePathAndName, out FormatDefinition definition)
        {
            XmlLoading.LoadDefinitions();
            string fileName = Path.GetFileNameWithoutExtension(filePathAndName);
            definition = Definitions.FirstOrDefault(def => fileName.Contains(def.FileType));
            return definition != null;
        }
        /// <summary>
        /// <para><returns>Returns a <see cref="FormatDefinition" /> for <paramref name="fileType" /></returns></para> 
        /// <para>Exceptions:<br/>
        /// <see cref="IOException" /><br/>
        /// <see cref="ArgumentException" /><br/>
        /// </para>
        /// </summary>
        public static FormatDefinition GetDefinition(string fileType)
        {
            XmlLoading.LoadDefinitions();
            FormatDefinition definition = Definitions.FirstOrDefault(def => def.FileType.Equals(fileType));
            if (definition == null) { throw new ArgumentException(string.Format("FileType '{0}' is not supported", fileType)); }
            return definition;
        }

        /// <summary>
        /// <para><returns>Test <paramref name="key" /> to be valid</returns></para> 
        /// </summary>
        public static bool IsKeyValid(string key)
        {
            return !string.IsNullOrEmpty(key);
        }
        /// <summary>
        /// <para><returns>Test <paramref name="value" /> to be valid</returns></para> 
        /// </summary>
        public static bool IsValueValid(string value)
        {
            return value != null;
        }
        /// <summary>
        /// <para><returns>Throws exception if <paramref name="blockType" /> is not a valid block type of <paramref name="definedBlockTypes" /></returns></para> 
        /// <para>
        /// Exceptions:<br/>
        /// <see cref="EcfFormatException" /><br/>
        /// </para>
        /// </summary>
        public static void CheckBlockType(string blockType, ReadOnlyCollection<BlockTypeDefinition> definedBlockTypes)
        {
            List<BlockTypeDefinition> mandatoryBlockTypes = definedBlockTypes.Where(type => !type.IsOptional).ToList();
            if (mandatoryBlockTypes.Count > 0 && !mandatoryBlockTypes.Any(type => type.Name.Equals(blockType)))
            {
                throw new EcfFormatException(EcfErrors.BlockTypeMissing, string.Format("found '{0}', expected: '{1}'", blockType, 
                    string.Join(", ", mandatoryBlockTypes.Select(type => type.Name).ToArray())));
            }
            if (!definedBlockTypes.Any(type => type.Name.Equals(blockType)))
            {
                throw new EcfFormatException(EcfErrors.BlockTypeUnknown, blockType);
            }
        }
        /// <summary>
        /// <para><returns>Throws exception if <paramref name="attributes" /> is not complete to <paramref name="definedAttributes" /></returns></para> 
        /// <para>
        /// Exceptions:<br/>
        /// <see cref="EcfFormatException" /><br/>
        /// </para>
        /// </summary>
        public static void CheckAttributes(List<EcfAttribute> attributes, ReadOnlyCollection<ItemDefinition> definedAttributes)
        {
            List<ItemDefinition> missingAttributes = definedAttributes.Where(defAttr => !defAttr.IsOptional && !attributes.Any(attr => attr.Key.Equals(defAttr.Name))).ToList();
            if (missingAttributes.Count > 0)
            {
                throw new EcfFormatException(EcfErrors.AttributeMissing, string.Join(", ", missingAttributes.Select(attr => attr.Name).ToArray()));
            }
            List<EcfAttribute> doubledAttributes = attributes.Except(attributes.Distinct(KeyItemComparer)).Cast<EcfAttribute>().ToList();
            if (doubledAttributes.Count > 0)
            {
                throw new EcfFormatException(EcfErrors.AttributeNotUnique, string.Join(", ", doubledAttributes.Select(param => param.Key).ToArray()));
            }
        }
        /// <summary>
        /// <para><returns>Throws exception if <paramref name="parameters" /> is not complete to <paramref name="definedParameters" /></returns></para> 
        /// <para>
        /// Exceptions:<br/>
        /// <see cref="EcfFormatException" /><br/>
        /// </para>
        /// </summary>
        public static void CheckParameters(List<EcfParameter> parameters, ReadOnlyCollection<ItemDefinition> definedParameters)
        {
            List<ItemDefinition> missingParameters = definedParameters.Where(defParam => !defParam.IsOptional && !parameters.Any(param => param.Key.Equals(defParam.Name))).ToList();
            if (missingParameters.Count > 0)
            {
                throw new EcfFormatException(EcfErrors.ParameterMissing, string.Join(", ", missingParameters.Select(param => param.Name).ToArray()));
            }
            List<EcfParameter> doubledParameters = parameters.Except(parameters.Distinct(KeyItemComparer)).Cast<EcfParameter>().ToList();
            if (doubledParameters.Count > 0)
            {
                throw new EcfFormatException(EcfErrors.ParameterNotUnique, string.Join(", ", doubledParameters.Select(param => param.Key).ToArray()));
            }
        }
        /// <summary>
        /// <para><returns>Throws exception if <paramref name="value" /> contains prohibited phrases from <paramref name="definition" /></returns></para> 
        /// <para>
        /// Exceptions:<br/>
        /// <see cref="EcfFormatException" /><br/>
        /// </para>
        /// </summary>
        public static void CheckValue(string value, FormatDefinition definition)
        {
            if (!IsValueValid(value)) { throw new EcfFormatException(EcfErrors.ValueInvalid, value); }
            List<string> prohibitedPhrases = definition.ProhibitedValuePhrases.Where(phrase => value.Contains(phrase)).ToList();
            if (prohibitedPhrases.Count > 0)
            {
                throw new EcfFormatException(EcfErrors.ValueContainsProhibitedPhrases, string.Join(", ", prohibitedPhrases));
            }
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

        private static class XmlLoading
        {
            private static class XmlSettings
            {
                public static string FolderName { get; } = "EcfParserSettings";
                public static string TemplateFileName { get; } = "EcfParserSettings_BlocksConfig.xml";
                public static string FileNamePattern { get; } = "*.xml";

                public static string XChapterRoot { get; } = "Settings";

                public static string XChapterFileConfig { get; } = "Config";
                public static string XChapterFormatting { get; } = "Formatting";
                public static string XChapterBlockTypePreMarks { get; } = "BlockTypePreMarks";
                public static string XChapterBlockTypePostMarks { get; } = "BlockTypePostMarks";
                public static string XChapterRootBlockTypes { get; } = "RootBlockTypes";
                public static string XChapterRootBlockAttributes { get; } = "RootBlockAttributes";
                public static string XChapterChildBlockTypes { get; } = "ChildBlockTypes";
                public static string XChapterChildBlockAttributes { get; } = "ChildBlockAttributes";
                public static string XChapterBlockParameters { get; } = "BlockParameters";
                public static string XChapterParameterAttributes { get; } = "ParameterAttributes";

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
                public static string XElementBlockReferenceSourceAttribute { get; } = "BlockReferenceSourceAttribute";
                public static string XElementBlockReferenceTargetAttribute { get; } = "BlockReferenceTargetAttribute";
                public static string XElementEscapeIdentifierPair { get; } = "EscapeIdentifierPair";
                public static string XElementParamter { get; } = "Param";

                public static string XAttributeType { get; } = "type";
                public static string XAttributeValue { get; } = "value";
                public static string XAttributeOpener { get; } = "opener";
                public static string XAttributeCloser { get; } = "closer";
                public static string XAttributeName { get; } = "name";
                public static string XAttributeOptional { get; } = "optional";
                public static string XAttributeHasValue { get; } = "hasValue";
                public static string XAttributeForceEscape { get; } = "forceEscape";
                public static string XAttributeInfo { get; } = "info";
            }
            private static XmlDocument XmlDoc { get; } = new XmlDocument();

            public static void LoadDefinitions()
            {
                if (!DefinitionsLoaded)
                {
                    Definitions.Clear();
                    Directory.CreateDirectory(XmlSettings.FolderName);
                    try
                    {
                        foreach (string filePathAndName in Directory.GetFiles(XmlSettings.FolderName, XmlSettings.FileNamePattern))
                        {
                            try
                            {
                                ReadDefinitionFile(filePathAndName);
                            }
                            catch(Exception ex)
                            {
                                throw new IOException(string.Format("Settings file '{0}' could not be loaded: {1}", filePathAndName, ex.Message));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new IOException(string.Format("Settings files could not be found: {0}", ex.Message));
                    }
                    if (Definitions.Count == 0)
                    {
                        string filePathAndName = "";
                        try
                        {
                            filePathAndName = Path.Combine(XmlSettings.FolderName, XmlSettings.TemplateFileName);
                            CreateXmlTemplate(filePathAndName);
                            ReadDefinitionFile(filePathAndName);
                        }
                        catch (Exception ex)
                        {
                            throw new IOException(string.Format("Template settings file '{0}' could not be loaded: {1}", filePathAndName, ex.Message));
                        }
                    }
                    DefinitionsLoaded = true;
                }
            }

            private static void ReadDefinitionFile(string filePathAndName)
            {
                XmlDoc.Load(filePathAndName);
                foreach (XmlNode configNode in XmlDoc.SelectNodes(string.Format("//{0}", XmlSettings.XChapterFileConfig)))
                {
                    string fileType = configNode.Attributes?.GetNamedItem(XmlSettings.XAttributeType)?.Value;
                    if (IsKeyValid(fileType) && Definitions.All(def => !def.FileType.Equals(fileType)))
                    {
                        Definitions.Add(BuildFormatDefinition(configNode));
                    }
                }
            }
            private static FormatDefinition BuildFormatDefinition(XmlNode configNode)
            {
                string fileType = configNode?.Attributes.GetNamedItem(XmlSettings.XAttributeType)?.Value;
                if (fileType == null) { throw new ArgumentException(string.Format("Attribute {0} not found", XmlSettings.XAttributeType)); }

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

                string itemSeperator = RepairXmlControlLiterals(formatterNode.SelectSingleNode(XmlSettings.XElementItemSeperator)?.Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);
                string itemValueSeperator = RepairXmlControlLiterals(formatterNode.SelectSingleNode(XmlSettings.XElementItemValueSeperator)?.Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);
                string valueSeperator = RepairXmlControlLiterals(formatterNode.SelectSingleNode(XmlSettings.XElementValueSeperator)?.Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);
                string valueGroupSeperator = RepairXmlControlLiterals(formatterNode.SelectSingleNode(XmlSettings.XElementValueGroupSeperator)?.Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);
                string valueFractionalSeperator = RepairXmlControlLiterals(formatterNode.SelectSingleNode(XmlSettings.XElementValueFractionalSeperator)?.Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);
                string magicSpacer = RepairXmlControlLiterals(formatterNode.SelectSingleNode(XmlSettings.XElementMagicSpacer)?.Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);
                string blockReferenceSourceAttribute = RepairXmlControlLiterals(formatterNode.SelectSingleNode(XmlSettings.XElementBlockReferenceSourceAttribute)?.Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);
                string blockReferenceTargetAttribute = RepairXmlControlLiterals(formatterNode.SelectSingleNode(XmlSettings.XElementBlockReferenceTargetAttribute)?.Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value);
                
                if (!IsKeyValid(itemSeperator)) { throw new ArgumentException(string.Format("Element {0} not valid", XmlSettings.XElementItemSeperator)); }
                if (!IsKeyValid(itemValueSeperator)) { throw new ArgumentException(string.Format("Element {0} not valid", XmlSettings.XElementItemValueSeperator)); }
                if (!IsKeyValid(valueSeperator)) { throw new ArgumentException(string.Format("Element {0} not valid", XmlSettings.XElementValueSeperator)); }
                if (!IsKeyValid(valueGroupSeperator)) { throw new ArgumentException(string.Format("Element {0} not valid", XmlSettings.XElementValueGroupSeperator)); }
                if (!IsKeyValid(valueFractionalSeperator)) { throw new ArgumentException(string.Format("Element {0} not valid", XmlSettings.XElementValueFractionalSeperator)); }
                if (!IsKeyValid(magicSpacer)) { throw new ArgumentException(string.Format("Element {0} not valid", XmlSettings.XElementMagicSpacer)); }

                List<MarkDefinition> blockTypePreMarks = BuildMarkList(configNode, XmlSettings.XChapterBlockTypePreMarks);
                List<MarkDefinition> blockTypePostMarks = BuildMarkList(configNode, XmlSettings.XChapterBlockTypePostMarks);
                List<BlockTypeDefinition> rootBlockTypes = BuildBlockTypeList(configNode, XmlSettings.XChapterRootBlockTypes);
                List<ItemDefinition> rootBlockAttributes = BuildItemList(configNode, XmlSettings.XChapterRootBlockAttributes);
                List<BlockTypeDefinition> childBlockTypes = BuildBlockTypeList(configNode, XmlSettings.XChapterChildBlockTypes);
                List<ItemDefinition> childBlockAttributes = BuildItemList(configNode, XmlSettings.XChapterChildBlockAttributes);
                List<ItemDefinition> blockParameters = BuildItemList(configNode, XmlSettings.XChapterBlockParameters);
                List<ItemDefinition> parameterAttributes = BuildItemList(configNode, XmlSettings.XChapterParameterAttributes);

                return new FormatDefinition(fileType,
                    singleLineCommentStarts, multiLineCommentPairs,
                    blockPairs, escapeIdentifierPairs, outerTrimmingPhrases,
                    itemSeperator, itemValueSeperator, valueSeperator, 
                    valueGroupSeperator, valueFractionalSeperator, magicSpacer,
                    blockReferenceSourceAttribute, blockReferenceTargetAttribute,
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
                    writer.WriteAttributeString(XmlSettings.XAttributeType, "BlocksConfig");
                    writer.WriteComment("Ecf Syntax Format Settings");
                    // Formatting
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
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementBlockReferenceSourceAttribute, "Ref");
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementBlockReferenceTargetAttribute, "Name");
                        writer.WriteComment("Copy Parameter if more needed, First is used at file write");
                        CreateXmlSpecificValueItem(writer, XmlSettings.XElementSingleLineCommentStart, "#");
                        CreateXmlPairValueItem(writer, XmlSettings.XElementBlockIdentifierPair, "{", "}");
                        CreateXmlPairValueItem(writer, XmlSettings.XElementEscapeIdentifierPair, "\"", "\"");
                        writer.WriteEndElement();
                    }
                    writer.WriteComment("File Specific Syntax Settings, Add more child-params if needed");
                    // premarks
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
                        writer.WriteStartElement(XmlSettings.XChapterRootBlockAttributes);;
                        CreateXmlParameterItem(writer, "Id", true, true, false);
                        CreateXmlParameterItem(writer, "Name", false, true, false);
                        CreateXmlParameterItem(writer, "Ref", true, true, false);
                        writer.WriteEndElement();
                    }
                    // Child block types
                    {
                        writer.WriteStartElement(XmlSettings.XChapterChildBlockTypes);
                        CreateXmlTypeItem(writer, "Child", true);
                        writer.WriteEndElement();
                    }
                    // child block Attributes
                    {
                        writer.WriteStartElement(XmlSettings.XChapterChildBlockAttributes);
                        CreateXmlParameterItem(writer, "DropOnDestroy", true, false, false);
                        writer.WriteEndElement();
                    }
                    // block parameters
                    {
                        writer.WriteStartElement(XmlSettings.XChapterBlockParameters);
                        CreateXmlParameterItem(writer, "Material", true, true, false);
                        CreateXmlParameterItem(writer, "Shape", true, true, false);
                        CreateXmlParameterItem(writer, "Mesh", true, true, false);
                        writer.WriteEndElement();
                    }
                    // parameter Attributes
                    {
                        writer.WriteStartElement(XmlSettings.XChapterParameterAttributes);
                        CreateXmlParameterItem(writer, "type", true, true, false);
                        CreateXmlParameterItem(writer, "display", true, true, false);
                        CreateXmlParameterItem(writer, "formatter", true, true, false);
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
                writer.WriteStartElement(XmlSettings.XElementParamter);
                writer.WriteAttributeString(XmlSettings.XAttributeValue, value);
                writer.WriteAttributeString(XmlSettings.XAttributeOptional, isOptional.ToString().ToLower());
                writer.WriteEndElement();
            }
            private static void CreateXmlTypeItem(XmlWriter writer, string name, bool isOptional)
            {
                writer.WriteStartElement(XmlSettings.XElementParamter);
                writer.WriteAttributeString(XmlSettings.XAttributeName, name);
                writer.WriteAttributeString(XmlSettings.XAttributeOptional, isOptional.ToString().ToLower());
                writer.WriteEndElement();
            }
            private static void CreateXmlParameterItem(XmlWriter writer, string name, bool isOptional, bool hasValue, bool isFordeEcaped)
            {
                writer.WriteStartElement(XmlSettings.XElementParamter);
                writer.WriteAttributeString(XmlSettings.XAttributeName, name);
                writer.WriteAttributeString(XmlSettings.XAttributeOptional, isOptional.ToString().ToLower());
                writer.WriteAttributeString(XmlSettings.XAttributeHasValue, hasValue.ToString().ToLower());
                writer.WriteAttributeString(XmlSettings.XAttributeForceEscape, isFordeEcaped.ToString().ToLower());
                writer.WriteAttributeString(XmlSettings.XAttributeInfo, "");
                writer.WriteEndElement();
            }

            private static List<string> BuildStringList(XmlNode formatterNode, string xElement)
            {
                return formatterNode.SelectNodes(xElement).Cast<XmlNode>().Select(node =>
                    RepairXmlControlLiterals(node.Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value)
                    ).Where(data => IsKeyValid(data)).ToList();
            }
            private static List<StringPairDefinition> BuildStringPairList(XmlNode formatterNode, string xElement)
            {
                return formatterNode.SelectNodes(xElement).Cast<XmlNode>().Select(node =>
                    new StringPairDefinition(
                        RepairXmlControlLiterals(node.Attributes?.GetNamedItem(XmlSettings.XAttributeOpener)?.Value),
                        RepairXmlControlLiterals(node.Attributes?.GetNamedItem(XmlSettings.XAttributeCloser)?.Value)
                    )
                ).Where(pair => pair.IsValid()).ToList();
            }
            private static List<MarkDefinition> BuildMarkList(XmlNode fileNode, string xChapter)
            {
                List<MarkDefinition> preMarks = fileNode.SelectSingleNode(xChapter)?.SelectNodes(XmlSettings.XElementParamter)?.Cast<XmlNode>().Select(node =>
                {
                    try
                    {
                        return new MarkDefinition(
                            RepairXmlControlLiterals(node.Attributes?.GetNamedItem(XmlSettings.XAttributeValue)?.Value),
                            node.Attributes?.GetNamedItem(XmlSettings.XAttributeOptional)?.Value
                            );
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }).Where(preMark => preMark != null).ToList();
                if (preMarks == null)
                {
                    preMarks = new List<MarkDefinition>();
                }
                return preMarks;
            }
            private static List<BlockTypeDefinition> BuildBlockTypeList(XmlNode fileNode, string xChapter)
            {
                List<BlockTypeDefinition> blockTypes = fileNode.SelectSingleNode(xChapter)?.SelectNodes(XmlSettings.XElementParamter)?.Cast<XmlNode>().Select(node =>
                {
                    try
                    {
                        return new BlockTypeDefinition(
                            node.Attributes?.GetNamedItem(XmlSettings.XAttributeName)?.Value,
                            node.Attributes?.GetNamedItem(XmlSettings.XAttributeOptional)?.Value
                            );
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }).Where(blockType => blockType != null).ToList();
                if (blockTypes == null)
                {
                    blockTypes = new List<BlockTypeDefinition>();
                }
                return blockTypes;
            }
            private static List<ItemDefinition> BuildItemList(XmlNode fileNode, string xChapter)
            {
                List<ItemDefinition> parameters = fileNode.SelectSingleNode(xChapter)?.SelectNodes(XmlSettings.XElementParamter)?.Cast<XmlNode>().Select(node =>
                {
                    try
                    {
                        return new ItemDefinition(
                            node.Attributes?.GetNamedItem(XmlSettings.XAttributeName)?.Value,
                            node.Attributes?.GetNamedItem(XmlSettings.XAttributeOptional)?.Value,
                            node.Attributes?.GetNamedItem(XmlSettings.XAttributeHasValue)?.Value,
                            node.Attributes?.GetNamedItem(XmlSettings.XAttributeForceEscape)?.Value,
                            node.Attributes?.GetNamedItem(XmlSettings.XAttributeInfo)?.Value
                            );
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }).Where(param => param != null).ToList();
                if (parameters == null)
                {
                    parameters = new List<ItemDefinition>();
                }
                return parameters;
            }
        }
    } 
    
    public class EgsEcfFile
    {
        public string FilePath { get; private set; } = null;
        public string FileName { get; private set; } = null;
        public Encoding FileEncoding { get; private set; } = null;
        public string NewLineCharacter { get; private set; } = null;
        public bool HasUnsavedData { get; private set; } = false;
        public FormatDefinition Definition { get; private set; } = null;

        /// <summary>
        /// <para><returns>Returns a list of ecf items with multiple sub-items.</returns><br/>
        /// <see cref="EcfComment" /><br/>
        /// <see cref="EcfBlock" /><br/>
        /// <see cref="EcfParameter" /><br/>
        /// </para> 
        /// </summary>
        public ReadOnlyCollection<EcfItem> ItemList { get; }
        /// <summary>
        /// <para>
        /// <returns>Returns a list of <see cref="EcfError" /> occured at content load, clears at save.</returns><br/>
        /// </para> 
        /// </summary>
        public ReadOnlyCollection<EcfError> ErrorList { get; }

        private List<EcfItem> InternalItemList { get; } = new List<EcfItem>();
        private List<EcfError> InternalErrorList { get; } = new List<EcfError>();
        private List<StackItem> Stack { get; } = new List<StackItem>();
        private StringBuilder EcfLineBuilder { get; } = new StringBuilder();

        /// <summary>
        /// <para><returns>Constructs a new <see cref="EgsEcfParser" /> and imports its content. <br/>
        /// Needs <see cref="FormatDefinition" /> from <see cref="GetDefinition" /> / <see cref="GetSupportedFileTypes" />.</returns></para>
        /// <para>
        /// Exceptions:<br/>
        /// <see cref="IOException" /><br/>
        /// <see cref="EcfFormatException" /><br/>
        /// </para>
        /// </summary>
        public EgsEcfFile(string filePathAndName, FormatDefinition definition)
        {
            ItemList = InternalItemList.AsReadOnly();
            ErrorList = InternalErrorList.AsReadOnly();
            FileName = Path.GetFileName(filePathAndName);
            FilePath = Path.GetDirectoryName(filePathAndName);
            Definition = new FormatDefinition(definition);
            Reload();
        }
        /// <summary>
        /// <para><returns>Constructs a new <see cref="EgsEcfParser" /> and imports its content. <br/>
        /// Trying to guess the needed <see cref="FormatDefinition" /> from the file name.</returns></para>
        /// <para>
        /// Exceptions:<br/>
        /// <see cref="IOException" /><br/>
        /// <see cref="EcfFormatException" /><br/>
        /// </para>
        /// </summary>
        public EgsEcfFile(string filePathAndName) : this(filePathAndName, GetDefinition(Path.GetFileNameWithoutExtension(filePathAndName)))
        {
            
        }

        /// <summary>
        /// <para><returns>Reloads the content data from the associated ecf file.</returns></para> 
        /// <para>
        /// Exceptions:<br/>
        /// <see cref="IOException" /><br/>
        /// <see cref="EcfFormatException" /><br/>
        /// </para>
        /// </summary>
        public void Reload()
        {
            string filePathAndName = Path.Combine(FilePath, FileName);
            try
            {
                FileEncoding = GetFileEncoding(filePathAndName);
                NewLineCharacter = GetNewLineChar(filePathAndName);
                using (StreamReader reader = new StreamReader(File.Open(filePathAndName, FileMode.Open, FileAccess.Read), FileEncoding))
                {
                    ParseEcfContent(reader);
                    HasUnsavedData = InternalErrorList.Count != 0;
                }
            }
            catch (Exception ex)
            {
                throw new IOException(string.Format("File {0} could not be loaded: {1}", filePathAndName, ex.Message));
            }
        }
        /// <summary>
        /// <para><returns>Saves the content data to the file.</returns></para> 
        /// <para>
        /// Exceptions:<br/>
        /// <see cref="IOException" /><br/>
        /// </para>
        /// </summary>
        public void Save()
        {
            Save(Path.Combine(FilePath, FileName));
        }
        /// <summary>
        /// <para><returns>Saves the content data to the file using a new path and name.</returns></para> 
        /// <para>
        /// Exceptions:<br/>
        /// <see cref="IOException" /><br/>
        /// </para>
        /// </summary>
        public void Save(string filePathAndName)
        {
            if (HasUnsavedData)
            {
                try
                {
                    string path = Path.GetDirectoryName(filePathAndName);
                    Directory.CreateDirectory(path);
                    using (StreamWriter writer = new StreamWriter(File.Open(filePathAndName, FileMode.Create, FileAccess.Write), FileEncoding))
                    {
                        writer.NewLine = NewLineCharacter;
                        CreateEcfContent(writer);
                    }
                    FileName = Path.GetFileName(filePathAndName);
                    FilePath = path;
                    HasUnsavedData = false;
                    InternalErrorList.Clear();
                }
                catch (Exception ex)
                {
                    throw new IOException(string.Format("File {0} could not be saved: {1}", filePathAndName, ex.Message));
                }
            }
        }
        /// <summary>
        /// <para><returns>Checks the definition against the file content and returns item definitions for items which are not present in the file content.</returns></para> 
        /// </summary>
        public ReadOnlyCollection<ItemDefinition> GetDeprecatedItemDefinitions()
        {
            List<ItemDefinition> deprecatedItems = new List<ItemDefinition>();
            List<ItemDefinition> definedRootBlockAttributes = Definition.RootBlockAttributes.ToList();
            List<ItemDefinition> definedChildBlockAttributes = Definition.ChildBlockAttributes.ToList();
            List<ItemDefinition> definedBlockParameters = Definition.BlockParameters.ToList();
            List<ItemDefinition> definedParameterAttributes = Definition.ParameterAttributes.ToList();
            foreach (EcfItem item in ItemList)
            {
                if (item is EcfBlock block)
                {
                    CheckDeprecatedItemDefinitions(block, definedRootBlockAttributes, definedChildBlockAttributes, definedBlockParameters, definedParameterAttributes);
                }
                if (definedRootBlockAttributes.Count == 0 && definedChildBlockAttributes.Count == 0 && 
                    definedBlockParameters.Count == 0 && definedParameterAttributes.Count == 0)
                {
                    break;
                }
            }
            deprecatedItems.AddRange(definedRootBlockAttributes);
            deprecatedItems.AddRange(definedChildBlockAttributes);
            deprecatedItems.AddRange(definedBlockParameters);
            deprecatedItems.AddRange(definedParameterAttributes);
            return deprecatedItems.AsReadOnly();
        }
        /// <summary>
        /// <para>Sets the <see cref="HasUnsavedData" /> flag for file change handling.</para> 
        /// </summary>
        public void SetUnsavedDataFlag()
        {
            if (!HasUnsavedData)
            {
                HasUnsavedData = true;
            }
        }
        /// <summary>
        /// <para>Adds a <see cref="EcfItem" /> to <see cref="ItemList" />.</para> 
        /// <para><returns>Returns true if <see cref="EcfItem" /> is not null.</returns></para> 
        /// </summary>
        public bool AddItem(EcfItem item)
        {
            if (item != null)
            {
                item.SetFile(this);
                InternalItemList.Add(item);
                return true;
            }
            return false;
        }
        /// <summary>
        /// <para>Adds a List of <see cref="EcfItem" /> to <see cref="ItemList" />.</para> 
        /// <para><returns>Returns count of added items if List nor item are null.</returns></para> 
        /// </summary>
        public int AddItems(List<EcfItem> items)
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

        // file formatting
        private Encoding GetFileEncoding(string filePathAndName)
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
        private string GetNewLineChar(string filePathAndName)
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
                            charBuffer.Append(buffer);
                        }
                        else if (buffer == '\r')
                        {
                            charBuffer.Append(buffer);
                            if (ReadChar(reader, out buffer) != -1)
                            {
                                if (buffer == '\n')
                                {
                                    charBuffer.Append(buffer);
                                }
                            }
                        }
                    }
                } while (!reader.EndOfStream && charBuffer.Length == 0);
            }
            return charBuffer.Length == 0 ? Environment.NewLine : charBuffer.ToString();
        }
        private int ReadChar(StreamReader reader, out char c)
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

        // ecf creating
        private void CreateEcfContent(StreamWriter writer)
        {
            int indent = 0;
            foreach (EcfItem item in ItemList)
            {
                if (item is EcfComment comment)
                {
                    CreateCommentLine(writer, comment, indent);
                }
                else if (item is EcfBlock rootBlock)
                {
                    CreateBlock(writer, rootBlock, indent);
                }
            }
        }
        private void CreateCommentLine(StreamWriter writer, EcfComment comment, int indent)
        {
            CreateIndent(EcfLineBuilder, indent);
            EcfLineBuilder.Append(Definition.WritingSingleLineCommentStart);
            EcfLineBuilder.Append(Definition.MagicSpacer);
            EcfLineBuilder.Append(string.Join(" / ", comment.Comments));
            writer.WriteLine(EcfLineBuilder.ToString());
        }
        private void CreateBlock(StreamWriter writer, EcfBlock block, int indent)
        {
            CreateBlockStartLine(writer, block, indent);
            indent++;
            foreach (EcfItem item in block.ChildItems)
            {
                if (item is EcfComment comment)
                {
                    CreateCommentLine(writer, comment, indent);
                }
                else if (item is EcfBlock childBlock)
                {
                    CreateBlock(writer, childBlock, indent);
                }
                else if (item is EcfParameter parameter)
                {
                    CreateParameterLine(writer, parameter, indent);
                }
            }
            indent--;
            CreateBlockEndLine(writer, indent);
        }
        private void CreateBlockStartLine(StreamWriter writer, EcfBlock block, int indent)
        {
            CreateIndent(EcfLineBuilder, indent);
            AppendBlockType(EcfLineBuilder, block);
            AppendAttributes(EcfLineBuilder, block.Attributes);
            AppendComments(EcfLineBuilder, block.Comments);
            writer.WriteLine(EcfLineBuilder.ToString());
        }
        private void CreateBlockEndLine(StreamWriter writer, int indent)
        {
            CreateIndent(EcfLineBuilder, indent);
            EcfLineBuilder.Append(Definition.WritingBlockIdentifierPair.Closer);
            writer.WriteLine(EcfLineBuilder.ToString());
        }
        private void CreateParameterLine(StreamWriter writer, EcfParameter parameter, int indent)
        {
            CreateIndent(EcfLineBuilder, indent);
            AppendParameter(EcfLineBuilder, parameter);
            AppendAttributes(EcfLineBuilder, parameter.Attributes);
            AppendComments(EcfLineBuilder, parameter.Comments);
            writer.WriteLine(EcfLineBuilder.ToString());
        }
        private void CreateIndent(StringBuilder lineBuilder, int indent)
        {
            lineBuilder.Clear();
            while (indent > 0)
            {
                lineBuilder.Append(Definition.MagicSpacer + Definition.MagicSpacer);
                indent--;
            }
        }
        private void AppendComments(StringBuilder lineBuilder, ReadOnlyCollection<string> comments)
        {
            if (comments.Count > 0)
            {
                lineBuilder.Append(Definition.MagicSpacer);
                lineBuilder.Append(Definition.WritingSingleLineCommentStart);
                lineBuilder.Append(Definition.MagicSpacer);
                lineBuilder.Append(string.Join(" / ", comments));
            }
        }
        private void AppendBlockType(StringBuilder lineBuilder, EcfBlock block)
        {
            lineBuilder.Append(Definition.WritingBlockIdentifierPair.Opener);
            lineBuilder.Append(Definition.MagicSpacer);
            lineBuilder.Append(block.TypePreMark ?? "");
            lineBuilder.Append(block.BlockDataType ?? "");
            if (block.Attributes.Count > 0)
            {
                lineBuilder.Append(block.TypePostMark ?? "");
            }
        }
        private void AppendAttributes(StringBuilder lineBuilder, ReadOnlyCollection<EcfAttribute> attributes)
        {
            List<string> attributeItems = attributes.Select(attr => CreateItem(attr)).ToList();
            lineBuilder.Append(string.Join((Definition.ItemSeperator + Definition.MagicSpacer), attributeItems));
        }
        private void AppendParameter(StringBuilder lineBuilder, EcfParameter parameter)
        {
            lineBuilder.Append(CreateItem(parameter));
            if (parameter.Attributes.Count > 0)
            {
                lineBuilder.Append(Definition.ItemSeperator);
                lineBuilder.Append(Definition.MagicSpacer);
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

                if (keyValueItem.IsForceEscaped || keyValueItem.IsUsingGroups() || keyValueItem.HasMultiValue())
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
        private void ParseEcfContent(StreamReader reader)
        {
            string lineData;
            int lineCount = 0;
            int level = 0;

            Stack.Clear();
            InternalItemList.Clear();
            InternalErrorList.Clear();
            List<string> comments = new List<string>();
            bool parameterLine;
            StringPairDefinition inCommentBlockPair = null;

            while (!reader.EndOfStream)
            {
                // interprete next line
                lineCount++;
                lineData = TrimOuterPhrases(reader.ReadLine());
                if (!lineData.Equals(string.Empty))
                {
                    // comments
                    comments.Clear();
                    comments.AddRange(ParseComments(lineData, out lineData, inCommentBlockPair, out inCommentBlockPair));
                    if (lineData.Equals(string.Empty))
                    {
                        EcfComment comment = new EcfComment(comments);
                        if (level > 0)
                        {
                            GetStackItem(level).Block.AddChild(comment);
                        }
                        else
                        {
                            AddItem(comment);
                        }
                        continue;
                    }
                    // Block opener
                    StringPairDefinition blockIdPair = Definition.BlockIdentifierPairs.FirstOrDefault(pair => lineData.StartsWith(pair.Opener));
                    if (blockIdPair != null)
                    {
                        // IsRoot Block
                        if (level < 1)
                        {
                            try
                            {
                                EcfBlock block = ParseRootBlock(lineData);
                                block.AddComments(comments);
                                level++;
                                AddStackItem(level, block, lineCount, lineData, blockIdPair);
                            }
                            catch (EcfFormatException ex)
                            {
                                InternalErrorList.Add(new EcfError(ex.EcfError, ex.TextData, null, lineCount.ToString()));
                            }
                        }
                        // IsChild Block
                        else
                        {
                            try
                            {
                                EcfBlock block = ParseChildBlock(lineData);
                                block.AddComments(comments);
                                level++;
                                AddStackItem(level, block, lineCount, lineData, blockIdPair);
                            }
                            catch (EcfFormatException ex)
                            {
                                InternalErrorList.Add(new EcfError(ex.EcfError, ex.TextData, null, lineCount.ToString()));
                            }
                        }
                    }
                    // parameter or block closer
                    else if (level > 0)
                    {
                        StackItem item = GetStackItem(level);
                        parameterLine = false;
                        // parameter
                        if (!item.BlockSymbolPair.Closer.Equals(lineData))
                        {
                            parameterLine = true;
                            EcfBlock block = item.Block;
                            try
                            {
                                EcfParameter parameter = ParseParameter(lineData);
                                parameter.AddComments(comments);
                                block.AddChild(parameter);
                            }
                            catch (EcfFormatException ex)
                            {
                                InternalErrorList.Add(new EcfError(ex.EcfError, ex.TextData, block.BuildIdentification(), lineCount.ToString()));
                            }
                        }
                        // block closer
                        if (lineData.EndsWith(item.BlockSymbolPair.Closer))
                        {
                            level--;
                            EcfBlock block = item.Block;
                            try
                            {
                                CheckParameters(block.ChildItems.Where(child => child is EcfParameter).Cast<EcfParameter>().ToList(), Definition.BlockParameters);
                                // comments
                                if (!parameterLine) { block.AddComments(comments); }
                                // append block to parent
                                if (level > 0)
                                {
                                    StackItem parent = GetStackItem(level);
                                    parent.Block.AddChild(block);
                                }
                                // append block to root list
                                else
                                {
                                    AddItem(block);
                                }
                            }
                            catch (EcfFormatException ex)
                            {
                                InternalErrorList.Add(new EcfError(ex.EcfError, ex.TextData, block.BuildIdentification(), item.LineNumber.ToString()));
                            }
                        }
                    }
                    // reporting unassigned line or unopend block
                    else
                    {
                        if (Definition.BlockIdentifierPairs.Any(pair => lineData.EndsWith(pair.Closer)))
                        {
                            InternalErrorList.Add(new EcfError(EcfErrors.BlockCloserWithoutOpener, lineData, null, lineCount.ToString()));
                        }
                        else
                        {
                            InternalErrorList.Add(new EcfError(EcfErrors.ParameterWithoutParent, lineData, null, lineCount.ToString()));
                        }

                    }
                }
            }
            // reporting unclosed blocks 
            while (level > 0)
            {
                StackItem item = GetStackItem(level);
                InternalErrorList.Add(new EcfError(EcfErrors.BlockOpenerWithoutCloser, item.LineData, item.Block.BuildIdentification(), item.LineNumber.ToString()));
                level--;
            }
        }
        private List<string> ParseComments(string inLineData, out string outLineData, StringPairDefinition inCommentBlockPair, out StringPairDefinition outCommentBlockPair)
        {
            List<string> comments = new List<string>();
            comments.AddRange(ParseBlockComment(inLineData, out inLineData, inCommentBlockPair, out inCommentBlockPair));
            comments.AddRange(ParseSingleLineComment(inLineData, out inLineData));
            comments.AddRange(ParseInLineComment(inLineData, out inLineData));
            comments.AddRange(ParseMultiLineComment(inLineData, out inLineData, inCommentBlockPair, out inCommentBlockPair));
            outLineData = inLineData;
            outCommentBlockPair = inCommentBlockPair;
            return comments;
        }
        private List<string> ParseBlockComment(string inLineData, out string outLineData, StringPairDefinition inCommentBlockPair, out StringPairDefinition outCommentBlockPair)
        {
            List<string> comments = new List<string>();
            if (inCommentBlockPair != null)
            {
                int end = inLineData.IndexOf(inCommentBlockPair.Closer);
                if (end >= 0)
                {
                    comments.Add(TrimComment(inLineData.Substring(0, end)));
                    inLineData = inLineData.Remove(0, end + inCommentBlockPair.Closer.Length).Trim();
                    inCommentBlockPair = null;
                }
                else
                {
                    comments.Add(TrimComment(inLineData));
                    inLineData = "";
                }
            }
            outLineData = inLineData;
            outCommentBlockPair = inCommentBlockPair;
            return comments;
        }
        private List<string> ParseSingleLineComment(string inLineData, out string outLineData)
        {
            List<string> comments = new List<string>();
            string singleLineMark = Definition.SingleLineCommentStarts.Where(mark => inLineData.IndexOf(mark) >= 0).OrderByDescending(mark => inLineData.IndexOf(mark)).FirstOrDefault();
            if (singleLineMark != null)
            {
                int start = inLineData.IndexOf(singleLineMark);
                comments.Add(TrimComment(inLineData.Substring(start).Remove(0, singleLineMark.Length)));
                inLineData = inLineData.Remove(start).Trim();
            }
            outLineData = inLineData;
            return comments;
        }
        private List<string> ParseInLineComment(string inLineData, out string outLineData)
        {
            List<string> comments = new List<string>();
            StringPairDefinition inLineMark = null;
            do
            {
                inLineMark = Definition.MultiLineCommentPairs.FirstOrDefault(pair => {
                    int start = inLineData.IndexOf(pair.Opener);
                    if (start >= 0)
                    {
                        int end = inLineData.IndexOf(pair.Closer, start);
                        if (end >= 0)
                        {
                            comments.Add(TrimComment(inLineData.Substring(start + pair.Opener.Length, end - start - pair.Opener.Length)));
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
        private List<string> ParseMultiLineComment(string inLineData, out string outLineData, StringPairDefinition inCommentBlockPair, out StringPairDefinition outCommentBlockPair)
        {
            List<string> comments = new List<string>();
            StringPairDefinition multiLineMark;
            multiLineMark = Definition.MultiLineCommentPairs.Where(pair => inLineData.IndexOf(pair.Closer) >= 0)
                .OrderByDescending(pair => inLineData.IndexOf(pair.Closer)).LastOrDefault();
            if (multiLineMark != null)
            {
                int end = inLineData.IndexOf(multiLineMark.Closer);
                if (end >= 0)
                {
                    comments.Add(TrimComment(inLineData.Substring(0, end)));
                    inLineData = inLineData.Remove(0, end + multiLineMark.Closer.Length).Trim();
                    inCommentBlockPair = null;
                }
            }
            multiLineMark = Definition.MultiLineCommentPairs.Where(pair => inLineData.IndexOf(pair.Opener) >= 0)
                .OrderByDescending(pair => inLineData.IndexOf(pair.Opener)).FirstOrDefault();
            if (multiLineMark != null)
            {
                int start = inLineData.IndexOf(multiLineMark.Opener);
                if (start >= 0)
                {
                    comments.Add(TrimComment(inLineData.Substring(start + multiLineMark.Opener.Length)));
                    inLineData = inLineData.Remove(start).Trim();
                    inCommentBlockPair = multiLineMark;
                }
            }
            outLineData = inLineData;
            outCommentBlockPair = inCommentBlockPair;
            return comments;
        }
        private EcfBlock ParseRootBlock(string lineData)
        {
            lineData = TrimPairs(lineData, Definition.BlockIdentifierPairs);
            string preMark = ParseBlockPreMark(lineData);
            string blockType = ParseBlockType(lineData, preMark, out string postMark);
            CheckBlockType(blockType, Definition.RootBlockTypes);
            lineData = RemoveBlockType(lineData, preMark, blockType, postMark);
            Queue<string> splittedItems = SplitEcfItems(lineData);
            List<EcfAttribute> attributes = ParseAttributes(splittedItems, Definition.RootBlockAttributes);
            return new EcfBlock(preMark, blockType, postMark, true, attributes, null);
        }
        private EcfBlock ParseChildBlock(string lineData)
        {
            lineData = TrimPairs(lineData, Definition.BlockIdentifierPairs);
            string blockType = ParseBlockType(lineData, null, out string postMark);
            if (blockType != null)
            {
                CheckBlockType(blockType, Definition.ChildBlockTypes);
                lineData = RemoveBlockType(lineData, null, blockType, postMark);
            }
            Queue<string> splittedlineData = SplitEcfItems(lineData);
            List<EcfAttribute> attributes = ParseAttributes(splittedlineData, Definition.ChildBlockAttributes);
            return new EcfBlock(null, blockType, postMark, false, attributes, null);
        }
        private EcfParameter ParseParameter(string lineData)
        {
            lineData = TrimPairs(lineData, Definition.BlockIdentifierPairs);
            Queue<string> splittedItems = SplitEcfItems(lineData);
            KeyValuePair<string, string> parameter = ParseParameterKeyValue(splittedItems, out bool isForceEscaped);
            List<EcfValueGroup> groups = ParseValues(parameter.Value);
            List<EcfAttribute> attributes = ParseAttributes(splittedItems, Definition.ParameterAttributes);
            return new EcfParameter(parameter.Key, groups, isForceEscaped, attributes);
        }
        private string ParseBlockPreMark(string lineData)
        {
            string preMark = Definition.BlockTypePreMarks.FirstOrDefault(mark => lineData.StartsWith(mark.Value))?.Value;
            List<MarkDefinition> mandatoryPreMarks = Definition.BlockTypePreMarks.Where(defPreMark => !defPreMark.IsOptional).ToList();
            if (mandatoryPreMarks.Count > 0 && !mandatoryPreMarks.Any(mark => mark.Value.Equals(preMark)))
            {
                throw new EcfFormatException(EcfErrors.BlockPreMarkMissing, string.Join(", ", mandatoryPreMarks.Select(marks => marks.Value).ToArray()));
            }
            return preMark;
        }
        private string ParseBlockType(string lineData, string preMark, out string postMark)
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
                    escapePair = Definition.EscapeIdentifiersPairs.FirstOrDefault(pair => pair.Opener.Equals(buffer));
                    if (escapePair == null && Definition.BlockTypePostMarks.Any(mark => mark.Value.Equals(buffer)))
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
        private List<EcfAttribute> ParseAttributes(Queue<string> splittedItems, ReadOnlyCollection<ItemDefinition> definedAttributes)
        {
            List<EcfAttribute> attributes = new List<EcfAttribute>();
            while (splittedItems.Count > 0)
            {
                string key = splittedItems.Dequeue();
                List<EcfValueGroup> groups = null;
                ItemDefinition definedAttribute = definedAttributes.FirstOrDefault(attribute => attribute.Name.Equals(key));
                if (definedAttribute == null)
                {
                    throw new EcfFormatException(EcfErrors.AttributeUnknown, key);
                }
                if (definedAttribute.HasValue)
                {
                    if (splittedItems.Count > 0)
                    {
                        groups = ParseValues(splittedItems.Dequeue());
                    }
                    else
                    {
                        throw new EcfFormatException(EcfErrors.ValueMissing, key);
                    }
                }
                attributes.Add(new EcfAttribute(key, groups, definedAttribute.IsForceEscaped));
            }
            CheckAttributes(attributes, definedAttributes);
            return attributes;
        }
        private KeyValuePair<string, string> ParseParameterKeyValue(Queue<string> splittedItems, out bool isForceEscaped)
        {
            string key = null;
            string value = null;
            isForceEscaped = false;
            if (splittedItems.Count > 0)
            {
                key = splittedItems.Dequeue();
                ItemDefinition definedParameter = Definition.BlockParameters.FirstOrDefault(parameter => parameter.Name.Equals(key));
                if (definedParameter == null)
                {
                    throw new EcfFormatException(EcfErrors.ParameterUnknown, key);
                }
                if (definedParameter.HasValue)
                {
                    isForceEscaped = definedParameter.IsForceEscaped;
                    if (splittedItems.Count > 0)
                    {
                        value = splittedItems.Dequeue();
                    }
                    else
                    {
                        throw new EcfFormatException(EcfErrors.ValueMissing, key);
                    }
                }
            }
            return new KeyValuePair<string, string>(key, value);
        }
        private List<EcfValueGroup> ParseValues(string itemValue)
        {
            List<EcfValueGroup> groups = new List<EcfValueGroup>();
            foreach (string groupValues in TrimPairs(itemValue, Definition.EscapeIdentifiersPairs).Split(Definition.ValueGroupSeperator.ToArray()))
            {
                groups.Add(new EcfValueGroup(groupValues.Trim().Split(Definition.ValueSeperator.ToArray()).Select(value => value.Trim()).ToList()));
            }
            return groups;
        }
        private string TrimStarts(string lineData, ReadOnlyCollection<string> definedStarts)
        {
            string startMark = null;
            do
            {
                startMark = definedStarts.FirstOrDefault(start => lineData.StartsWith(start));
                if (startMark != null)
                {
                    lineData = lineData.Remove(0, startMark.Length).Trim();
                }
            } while (startMark != null);
            return lineData.Trim();
        }
        private string TrimPairs(string lineData, ReadOnlyCollection<StringPairDefinition> definedIdentifiers)
        {
            StringPairDefinition blockPair = null;
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
            return lineData.Trim();
        }
        private string TrimComment(string comment)
        {
            comment = TrimStarts(comment.Trim(), Definition.SingleLineCommentStarts).Trim();
            comment = TrimPairs(comment, Definition.MultiLineCommentPairs).Trim();
            return comment;
        }
        private string TrimOuterPhrases(string lineData)
        {
            string foundPhrase;
            do
            {
                foundPhrase = Definition.OuterTrimmingPhrases.FirstOrDefault(phrase => lineData.StartsWith(phrase));
                if (foundPhrase != null)
                {
                    lineData = lineData.Remove(0, foundPhrase.Length).Trim();
                }
            } while (foundPhrase != null);
            do
            {
                foundPhrase = Definition.OuterTrimmingPhrases.FirstOrDefault(phrase => lineData.EndsWith(phrase));
                if (foundPhrase != null)
                {
                    lineData = lineData.Remove(lineData.Length - foundPhrase.Length, foundPhrase.Length).Trim();
                }
            } while (foundPhrase != null);
            return lineData.Trim();
        }
        private string RemoveBlockType(string lineData, string preMark, string blockType, string postMark)
        {
            return lineData.Remove(0, (preMark?.Length ?? 0) + (blockType?.Length ?? 0) + (postMark?.Length ?? 0)).Trim();
        }
        private Queue<string> SplitEcfItems(string lineData)
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
                    escapePair = Definition.EscapeIdentifiersPairs.FirstOrDefault(pair => pair.Opener.Equals(buffer));
                    if (escapePair == null)
                    {
                        split = ((buffer == Definition.ItemSeperator) || (buffer == Definition.ItemValueSeperator));
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
        private void CheckDeprecatedItemDefinitions(EcfBlock block, List<ItemDefinition> definedRootBlockAttributes, List<ItemDefinition> definedChildBlockAttributes,
            List<ItemDefinition> definedBlockParameters, List<ItemDefinition> definedParameterAttributes)
        {
            if (block.IsRoot())
            {
                if (definedRootBlockAttributes.Count > 0)
                {
                    definedRootBlockAttributes.RemoveAll(defAttr => block.Attributes.Any(attr => defAttr.Name.Equals(attr.Key)));
                }
            }
            else
            {
                if (definedChildBlockAttributes.Count > 0)
                {
                    definedChildBlockAttributes.RemoveAll(defAttr => block.Attributes.Any(attr => defAttr.Name.Equals(attr.Key)));
                }
            }
            foreach (EcfItem subItem in block.ChildItems)
            {
                if (subItem is EcfBlock subBlock)
                {
                    CheckDeprecatedItemDefinitions(subBlock, definedRootBlockAttributes, definedChildBlockAttributes, definedBlockParameters, definedParameterAttributes);
                }
                else if (subItem is EcfParameter parameter)
                {
                    if (definedBlockParameters.Count > 0)
                    {
                        definedBlockParameters.RemoveAll(defParam => defParam.Name.Equals(parameter.Key));
                    }
                    if (definedParameterAttributes.Count > 0)
                    {
                        definedParameterAttributes.RemoveAll(defAttr => parameter.Attributes.Any(attr => defAttr.Name.Equals(attr.Key)));
                    }
                }
                if (definedRootBlockAttributes.Count == 0 && definedChildBlockAttributes.Count == 0 &&
                    definedBlockParameters.Count == 0 && definedParameterAttributes.Count == 0)
                {
                    break;
                }
            }
        }

        // stack data handling
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
        private void AddStackItem(int level, EcfBlock blockNode, int lineNumber, string lineData, StringPairDefinition blockSymbolPair)
        {
            if (Stack.Count < level)
            {
                Stack.Add(new StackItem(blockNode, lineNumber, lineData, blockSymbolPair));
            }
            else
            {
                Stack[level - 1] = new StackItem(blockNode, lineNumber, lineData, blockSymbolPair);
            }
        }
        private StackItem GetStackItem(int level)
        {
            return Stack[level - 1];
        }
    }

    // Ecf Error Handling
    public enum EcfErrors
    {
        Unknown,
        KeyNullOrEmpty,
        GroupIndexInvalid,

        BlockOpenerWithoutCloser,
        BlockCloserWithoutOpener,
        BlockTypeUnknown,
        BlockPreMarkMissing,
        BlockTypeMissing,

        ParameterUnknown,
        ParameterWithoutParent,
        ParameterMissing,
        ParameterNotUnique,

        AttributeUnknown,
        AttributeMissing,
        AttributeNotUnique,

        ValueMissing,
        ValueInvalid,
        ValueIndexInvalid,
        ValueContainsProhibitedPhrases,
    }
    public class EcfError
    {
        public string BlockName { get; }
        public string LineInFile { get; }
        public EcfErrors Error { get; }
        public string Data { get; }

        public EcfError(EcfErrors error, string data, string blockName, string lineInFile)
        {
            Error = error;
            Data = data;
            BlockName = blockName;
            LineInFile = lineInFile;
        }

        public override string ToString()
        {
            return string.Format("Error '{0}' in Line '{1}' at '{2}', parsed content: '{3}'", Error, LineInFile ?? "unknown", BlockName ?? "unknown", Data ?? "unknown");
        }
    }
    public class EcfFormatException : Exception
    {
        public EcfErrors EcfError { get; }
        public string TextData { get; }


        public EcfFormatException() : base(ToString(EcfErrors.Unknown, ""))
        {
            EcfError = EcfErrors.Unknown;
            TextData = "";
        }

        public EcfFormatException(EcfErrors ecfError) : base(ToString(ecfError, ""))
        {
            EcfError = ecfError;
            TextData = "";
        }

        public EcfFormatException(EcfErrors ecfError, string textData) : base(ToString(ecfError, textData))
        {
            EcfError = ecfError;
            TextData = textData ?? "null";
        }

        public EcfFormatException(EcfErrors ecfError, string textData, Exception inner) : base(ToString(ecfError, textData), inner)
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

    // ecf data Structure Classes
    public abstract class EcfItem
    {
        private EgsEcfFile EcfFile { get; set; } = null;
        public EcfItem Parent { get; private set; } = null;
        public int StructureLevel { get; private set; } = -1;

        public EcfItem()
        {
            
        }

        public abstract override string ToString();
        public bool IsRoot()
        {
            return Parent == null;
        }
        public EgsEcfFile FindFile()
        {
            if (IsRoot())
            {
                return EcfFile;
            }
            else
            {
                return Parent.FindFile();
            }
        }
        public void SetFile(EgsEcfFile file)
        {
            EcfFile = file;
        }
        public void UpdateStructureData(EcfItem parent, int level)
        {
            Parent = parent;
            StructureLevel = level;
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
        public int GetIndexInStructureLevel<T>() where T : EcfItem
        {
            ReadOnlyCollection<EcfItem> items;
            if (IsRoot())
            {
                items = EcfFile.ItemList;
            }
            else
            {
                items =(Parent as EcfBlock)?.ChildItems;
            }
            return items?.Where(child => child is T).Cast<T>().ToList().IndexOf((T)this) ?? -1;
        }
    }
    public abstract class EcfCommentItem : EcfItem
    {
        private List<string> InternalComments { get; } = new List<string>();
        public ReadOnlyCollection<string> Comments { get; }

        public EcfCommentItem() : base()
        {
            Comments = InternalComments.AsReadOnly();
        }

        public abstract override string ToString();
        public bool AddComment(string text)
        {
            if (IsKeyValid(text))
            {
                InternalComments.Add(text);
                FindFile()?.SetUnsavedDataFlag();
                return true;
            }
            return false;
        }
        public int AddComments(List<string> comments)
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
    }
    public abstract class EcfKeyValueItem : EcfCommentItem
    {
        public string Key { get; }
        public bool IsForceEscaped { get; }
        private List<EcfValueGroup> InternalValueGroups { get; } = new List<EcfValueGroup>();
        public ReadOnlyCollection<EcfValueGroup> ValueGroups { get; }

        public EcfKeyValueItem(string key, bool isForceEscaped) : base()
        {
            if (!IsKeyValid(key)) { throw new EcfFormatException(EcfErrors.KeyNullOrEmpty, GetType().Name); }
            Key = key;
            IsForceEscaped = isForceEscaped;
            ValueGroups = InternalValueGroups.AsReadOnly();
        }

        public abstract override string ToString();
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
            if (InternalValueGroups.Count == 0) { InternalValueGroups.Add(new EcfValueGroup()); }
            return AddValue(value, 0);
        }
        public bool AddValue(string value, int groupIndex)
        {
            if (groupIndex < 0 || groupIndex >= InternalValueGroups.Count) { throw new EcfFormatException(EcfErrors.GroupIndexInvalid, groupIndex.ToString()); }
            return InternalValueGroups[groupIndex].AddValue(value);
        }
        public int AddValues(List<string> values)
        {
            if (InternalValueGroups.Count == 0) { InternalValueGroups.Add(new EcfValueGroup()); }
            return AddValues(values, 0);
        }
        public int AddValues(List<string> values, int groupIndex)
        {
            if (groupIndex < 0 || groupIndex >= InternalValueGroups.Count) { throw new EcfFormatException(EcfErrors.GroupIndexInvalid, groupIndex.ToString()); }
            return InternalValueGroups[groupIndex].AddValues(values);
        }
        public bool AddValueGroup(EcfValueGroup valueGroup)
        {
            if (valueGroup != null) {
                valueGroup.UpdateStructureData(this, StructureLevel);
                InternalValueGroups.Add(valueGroup);
                FindFile()?.SetUnsavedDataFlag();
                return true;
            }
            return false;
        }
        public int AddValueGroups(List<EcfValueGroup> valueGroups)
        {
            int count = 0;
            valueGroups?.ForEach(group => {
                if (AddValueGroup(group))
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
            if (groupIndex < 0 || groupIndex >= InternalValueGroups.Count) { throw new EcfFormatException(EcfErrors.GroupIndexInvalid, groupIndex.ToString()); }
            if (valueIndex < 0 || valueIndex >= InternalValueGroups[groupIndex].Values.Count) { throw new EcfFormatException(EcfErrors.ValueIndexInvalid, valueIndex.ToString()); }
            return InternalValueGroups[groupIndex].Values[valueIndex];
        }
        public int ReplaceAllValues(string oldValue, string newValue)
        {
            int count = 0;
            InternalValueGroups.ForEach(group =>
            {
                count += group.ReplaceAllValues(oldValue, newValue);
            });
            count += CleanUpGroups();
            return count;
        }
        public bool ReplaceFirstValue(string oldValue, string newValue)
        {
            bool anyChange = InternalValueGroups.FirstOrDefault(group => group.Values.Contains(oldValue))?.ReplaceFirstValue(oldValue, newValue) ?? false;
            if (CleanUpGroups() > 0)
            {
                anyChange = true;
            }
            return anyChange;
        }
        public bool ReplaceValue(int valueIndex, int groupIndex, string newValue)
        {
            if (groupIndex < 0 || groupIndex >= InternalValueGroups.Count) { throw new EcfFormatException(EcfErrors.GroupIndexInvalid, groupIndex.ToString()); }
            bool anyChange = InternalValueGroups[groupIndex].ReplaceValue(valueIndex, newValue);
            if (CleanUpGroups() > 0)
            {
                anyChange = true;
            }
            return anyChange;
        }
        public int IndexOf(EcfValueGroup group)
        {
            return InternalValueGroups.IndexOf(group);
        }

        private int CleanUpGroups()
        {
            int count = InternalValueGroups.RemoveAll(group => group.Values.Count == 0);
            if (count > 0)
            {
                FindFile()?.SetUnsavedDataFlag();
            }
            return count;
        }
    }
    public class EcfAttribute : EcfKeyValueItem
    {
        private EcfAttribute(string key, bool isForceEscaped) : base(key, isForceEscaped)
        {
            
        }
        public EcfAttribute(string key, string value, bool isForceEscaped) : this(key, isForceEscaped)
        {
            AddValue(value);
        }
        public EcfAttribute(string key, List<string> values, bool isForceEscaped) : this(key, isForceEscaped)
        {
            AddValues(values);
        }
        public EcfAttribute(string key, List<EcfValueGroup> valueGroups, bool isForceEscaped) : this(key, isForceEscaped)
        {
            AddValueGroups(valueGroups);
        }

        public override string ToString()
        {
            return string.Format("Attribute with key: '{0}' and values: '{1}'", Key, ValueGroups.Sum(group => group.Values.Count).ToString());
        }
    }
    public class EcfParameter : EcfKeyValueItem
    {
        private List<EcfAttribute> InternalAttributes { get; } = new List<EcfAttribute>();
        public ReadOnlyCollection<EcfAttribute> Attributes { get; }

        private EcfParameter(string key, bool isForceEscaped) : base(key, isForceEscaped)
        {
            Attributes = InternalAttributes.AsReadOnly();
        }
        public EcfParameter(string key, string value, bool isForceEscaped) : this(key, isForceEscaped)
        {
            AddValue(value);
        }
        public EcfParameter(string key, List<string> values, bool isForceEscaped, List<EcfAttribute> attributes) : this(key, isForceEscaped)
        {
            AddValues(values);
            AddAttributes(attributes);
        }
        public EcfParameter(string key, List<EcfValueGroup> valueGroups, bool isForceEscaped, List<EcfAttribute> attributes) : this(key, isForceEscaped)
        {
            AddValueGroups(valueGroups);
            AddAttributes(attributes);
        }
        
        public override string ToString()
        {
            return string.Format("Parameter with key: '{0}', values: '{1}' and attributes: '{2}'", Key, ValueGroups.Sum(group => group.Values.Count).ToString(), Attributes.Count);
        }
        public bool AddAttribute(EcfAttribute attribute)
        {
            if (attribute != null)
            {
                attribute.UpdateStructureData(this, StructureLevel);
                InternalAttributes.Add(attribute);
                FindFile()?.SetUnsavedDataFlag();
                return true;
            }
            return false;
        }
        public int AddAttributes(List<EcfAttribute> attributes)
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
    }
    public class EcfBlock : EcfCommentItem
    {
        public string TypePreMark { get; }
        public string BlockDataType { get; }
        public string TypePostMark { get; }

        private List<EcfAttribute> InternalAttributes { get; } = new List<EcfAttribute>();
        public ReadOnlyCollection<EcfAttribute> Attributes { get; }
        private List<EcfItem> InternalChildItems { get; } = new List<EcfItem>();
        public ReadOnlyCollection<EcfItem> ChildItems { get; }

        public EcfBlock(string preMark, string blockDataType, string postMark, bool isRoot) : base()
        {
            if (isRoot) {
                if (!IsKeyValid(blockDataType)) { throw new EcfFormatException(EcfErrors.KeyNullOrEmpty, "RootBlock DataType"); }
                if (!IsKeyValid(postMark)) { throw new EcfFormatException(EcfErrors.KeyNullOrEmpty, "RootBlock TypePostMark"); }
            }
            Attributes = InternalAttributes.AsReadOnly();
            ChildItems = InternalChildItems.AsReadOnly();

            TypePreMark = preMark;
            BlockDataType = blockDataType;
            TypePostMark = postMark;
        }
        public EcfBlock(string preMark, string blockType, string postMark, bool isRoot, List<EcfAttribute> attributes, List<EcfItem> childItems)
            : this(preMark, blockType, postMark, isRoot)
        {
            AddAttributes(attributes);
            AddChilds(childItems);
        }

        public override string ToString()
        {
            return string.Format("Block with preMark: '{0}', blockDataType: '{1}', name: '{4}', items: '{2}', attributes: '{3}'",
                TypePreMark, BlockDataType, ChildItems.Count, Attributes.Count, GetAttributeFirstValue("Name"));
        }
        public bool AddChild(EcfItem childItem)
        {
            if (childItem != null)
            {
                childItem.UpdateStructureData(this, StructureLevel + 1);
                InternalChildItems.Add(childItem);
                FindFile()?.SetUnsavedDataFlag();
                return true;
            }
            return false;
        }
        public int AddChilds(List<EcfItem> childItems)
        {
            int count = 0;
            childItems?.ForEach(item => { 
                if (AddChild(item))
                {
                    count++;
                } 
            });
            return count;
        }
        public bool AddAttribute(EcfAttribute attribute)
        {
            if (attribute != null)
            {
                attribute.UpdateStructureData(this, StructureLevel);
                InternalAttributes.Add(attribute);
                FindFile()?.SetUnsavedDataFlag();
                return true;
            }
            return false;
        }
        public int AddAttributes(List<EcfAttribute> attributes)
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
        public string GetAttributeFirstValue(string attrName)
        {
            return InternalAttributes.FirstOrDefault(attr => attr.Key.Equals(attrName))?.GetFirstValue();
        }
        public bool HasAttribute(string attrName)
        {
            return InternalAttributes.Any(attr => attr.Key.Equals(attrName));
        }
        public bool HasAttributeValue(string attrValue)
        {
            return InternalAttributes.Any(attr => attr.ValueGroups.Any(group => group.Values.Any(value => value.Equals(attrValue))));
        }
        public string BuildIdentification()
        {
            StringBuilder identification = new StringBuilder(BlockDataType);
            if (IsRoot())
            {
                string id = GetAttributeFirstValue("Id");
                if (id != null) {
                    identification.Append(", Id: ");
                    identification.Append(id);
                }
                string name = GetAttributeFirstValue("Name");
                if (name != null) {
                    identification.Append(", Name: ");
                    identification.Append(name);
                }
            }
            else
            {
                identification.Append(GetIndexInStructureLevel<EcfBlock>());
                if (Attributes.Count > 0)
                {
                    identification.Append(", ");
                    identification.Append(string.Join(", ", Attributes.Select(attr => attr.Key)));
                }
                
            }
            return identification.ToString();
        }
    }
    public class EcfComment : EcfCommentItem
    {
        public EcfComment(string comment) : base()
        {
            AddComment(comment);
        }
        public EcfComment(List<string> comments) : base()
        {
            AddComments(comments);
        }

        public override string ToString()
        {
            return string.Format("Comment with '{0}'", string.Join(" / ", Comments));
        }
    }
    public class EcfValueGroup : EcfItem
    {
        private List<string> InternalValues { get; } = new List<string>();
        public ReadOnlyCollection<string> Values { get; }

        public EcfValueGroup() : base()
        {
            Values = InternalValues.AsReadOnly();
        }
        public EcfValueGroup(string value) : this()
        {
            AddValue(value);
        }
        public EcfValueGroup(List<string> values) : this()
        {
            AddValues(values);
        }

        public override string ToString()
        {
            return string.Format("ValueGroup with '{0}'", string.Join(" / ", InternalValues));
        }
        public bool AddValue(string value)
        {
            if (IsValueValid(value)) { 
                InternalValues.Add(value);
                FindFile()?.SetUnsavedDataFlag();
                return true;
            }
            return false;
        }
        public int AddValues(List<string> values)
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
        public bool ReplaceValue(int valueIndex, string newValue)
        {
            if (valueIndex < 0 || valueIndex >= InternalValues.Count) { throw new EcfFormatException(EcfErrors.ValueIndexInvalid, valueIndex.ToString()); }
            if (newValue != null)
            {
                InternalValues[valueIndex] = newValue;
            }
            else
            {
                InternalValues.RemoveAt(valueIndex);
            }
            FindFile()?.SetUnsavedDataFlag();
            return true;
        }
        public bool ReplaceFirstValue(string oldValue, string newValue)
        {
            int index = InternalValues.IndexOf(oldValue);
            if (index >= 0)
            {
                ReplaceValue(index, newValue);
                return true;
            }
            return false;
        }
        public int ReplaceAllValues(string oldValue, string newValue)
        {
            int count = 0;
            int index;
            do
            {
                index = InternalValues.IndexOf(oldValue);
                if (index >= 0)
                {
                    ReplaceValue(index, newValue);
                    count++;
                }
            }
            while (index >= 0);
            return count;
        }
    }

    // definition data structures
    public class FormatDefinition
    {
        public string FileType { get; }

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
        public string BlockReferenceSourceAttribute { get; }
        public string BlockReferenceTargetAttribute { get; }

        public ReadOnlyCollection<string> ProhibitedValuePhrases { get; }

        public ReadOnlyCollection<MarkDefinition> BlockTypePreMarks { get; }
        public ReadOnlyCollection<MarkDefinition> BlockTypePostMarks { get; }
        public ReadOnlyCollection<BlockTypeDefinition> RootBlockTypes { get; }
        public ReadOnlyCollection<ItemDefinition> RootBlockAttributes { get; }
        public ReadOnlyCollection<BlockTypeDefinition> ChildBlockTypes { get; }
        public ReadOnlyCollection<ItemDefinition> ChildBlockAttributes { get; }
        public ReadOnlyCollection<ItemDefinition> BlockParameters { get; }
        public ReadOnlyCollection<ItemDefinition> ParameterAttributes { get; }

        public string WritingSingleLineCommentStart { get; }
        public StringPairDefinition WritingBlockIdentifierPair { get;}
        public StringPairDefinition WritingEscapeIdentifiersPair { get; }

        public FormatDefinition(string fileType,
            List<string> singleLineCommentStarts, List<StringPairDefinition> multiLineCommentPairs,
            List<StringPairDefinition> blockPairs, List<StringPairDefinition> escapeIdentifierPairs, List<string> outerTrimmingPhrases,
            string itemSeperator, string itemValueSeperator, string valueSeperator, 
            string valueGroupSeperator, string valueFractionalSeperator, string magicSpacer,
            string blockReferenceSourceAttribute, string blockReferenceTargetAttribute,
            List<MarkDefinition> blockTypePreMarks, List<MarkDefinition> blockTypePostMarks,
            List<BlockTypeDefinition> rootBlockTypes, List<ItemDefinition> rootBlockAttributes,
            List<BlockTypeDefinition> childBlockTypes, List<ItemDefinition> childBlockAttributes,
            List<ItemDefinition> blockParameters, List<ItemDefinition> parameterAttributes)
        {
            FileType = fileType;

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
            BlockReferenceSourceAttribute = blockReferenceSourceAttribute;
            BlockReferenceTargetAttribute = blockReferenceTargetAttribute;

            HashSet<string> prohibitedPhrases = new HashSet<string>();
            foreach (string start in SingleLineCommentStarts) { prohibitedPhrases.Add(start); }
            foreach (StringPairDefinition pair in MultiLineCommentPairs) { prohibitedPhrases.Add(pair.Opener); prohibitedPhrases.Add(pair.Closer); }
            foreach (StringPairDefinition pair in BlockIdentifierPairs) { prohibitedPhrases.Add(pair.Opener); prohibitedPhrases.Add(pair.Closer); }
            foreach (StringPairDefinition pair in EscapeIdentifiersPairs) { prohibitedPhrases.Add(pair.Opener); prohibitedPhrases.Add(pair.Closer); }
            prohibitedPhrases.Add(ItemSeperator);
            prohibitedPhrases.Add(ItemValueSeperator);
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
            FileType = template.FileType;

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
            BlockReferenceSourceAttribute = template.BlockReferenceSourceAttribute;
            BlockReferenceTargetAttribute = template.BlockReferenceTargetAttribute;

            ProhibitedValuePhrases = template.ProhibitedValuePhrases.ToList().AsReadOnly();

            BlockTypePreMarks = template.BlockTypePreMarks.Select(mark => new MarkDefinition(mark)).ToList().AsReadOnly();
            BlockTypePostMarks = template.BlockTypePostMarks.Select(mark => new MarkDefinition(mark)).ToList().AsReadOnly();
            RootBlockTypes = template.RootBlockTypes.Select(type => new BlockTypeDefinition(type)).ToList().AsReadOnly();
            RootBlockAttributes = template.RootBlockAttributes.Select(item => new ItemDefinition(item)).ToList().AsReadOnly();
            ChildBlockTypes = template.ChildBlockTypes.Select(type => new BlockTypeDefinition(type)).ToList().AsReadOnly();
            ChildBlockAttributes = template.ChildBlockAttributes.Select(item => new ItemDefinition(item)).ToList().AsReadOnly();
            BlockParameters = template.BlockParameters.Select(item => new ItemDefinition(item)).ToList().AsReadOnly();
            ParameterAttributes = template.ParameterAttributes.Select(item => new ItemDefinition(item)).ToList().AsReadOnly();

            WritingSingleLineCommentStart = template.WritingSingleLineCommentStart;
            WritingBlockIdentifierPair = template.WritingBlockIdentifierPair;
            WritingEscapeIdentifiersPair = template.WritingEscapeIdentifiersPair;
        }
    }
    public class MarkDefinition
    {
        public string Value { get; }
        public bool IsOptional { get; }

        public MarkDefinition(string value, string isOptional)
        {
            if (!IsKeyValid(value)) { throw new ArgumentException(string.Format("'{0}' is not a valid 'value' parameter", value)); }
            if (!bool.TryParse(isOptional, out bool optional)) { throw new ArgumentException(string.Format("'{0}' is not a valid 'isOptional' parameter", isOptional)); }
            Value = value;
            IsOptional = optional;
        }
        public MarkDefinition(MarkDefinition template)
        {
            Value = template.Value;
            IsOptional = template.IsOptional;
        }
    }
    public class BlockTypeDefinition
    {
        public string Name { get; }
        public bool IsOptional { get; }

        public BlockTypeDefinition(string name, string isOptional)
        {
            if (!IsKeyValid(name)) { throw new ArgumentException(string.Format("'{0}' is not a valid 'name' parameter", name)); }
            if (!bool.TryParse(isOptional, out bool optional)) { throw new ArgumentException(string.Format("'{0}' is not a valid 'isOptional' parameter", isOptional)); }
            Name = name;
            IsOptional = optional;
        }
        public BlockTypeDefinition(BlockTypeDefinition template)
        {
            Name = template.Name;
            IsOptional = template.IsOptional;
        }
    }
    public class ItemDefinition
    {
        public string Name { get; }
        public bool IsOptional { get; }
        public bool HasValue { get; }
        public bool IsForceEscaped { get; }
        public string Info { get; }

        public ItemDefinition(string name, string isOptional, string hasValue, string isForceEscaped, string info)
        {
            if (!IsKeyValid(name)) { throw new ArgumentException(string.Format("'{0}' is not a valid 'name' parameter", name)); }
            if (!bool.TryParse(isOptional, out bool optional)) { throw new ArgumentException(string.Format("'{0}' is not a valid 'isOptional' parameter", isOptional)); }
            if (!bool.TryParse(hasValue, out bool value)) { throw new ArgumentException(string.Format("'{0}' is not a valid 'hasValue' parameter", hasValue)); }
            if (!bool.TryParse(isForceEscaped, out bool forceEscaped)) { throw new ArgumentException(string.Format("'{0}' is not a valid 'forceEscape' parameter", isForceEscaped)); }
            Name = name;
            IsOptional = optional;
            HasValue = value;
            IsForceEscaped = forceEscaped;
            Info = info ?? "";
        }
        public ItemDefinition(ItemDefinition template)
        {
            Name = template.Name;
            IsOptional = template.IsOptional;
            HasValue = template.HasValue;
            IsForceEscaped = template.IsForceEscaped;
            Info = template.Info;
        }

        public override string ToString()
        {
            return string.Format("ItemDefinition: name: {0}, isOptional: {1}, hasValue: {2}, forceEscaped: {3}", Name, IsOptional, HasValue, IsForceEscaped);
        }
    }
    public class StringPairDefinition
    {
        public string Opener { get; }
        public string Closer { get; }
        public StringPairDefinition(string opener, string closer)
        {
            Opener = opener;
            Closer = closer;
        }
        public StringPairDefinition(StringPairDefinition template)
        {
            Opener = template.Opener;
            Closer = template.Closer;
        }
        public bool IsValid()
        {
            return IsKeyValid(Opener) && IsKeyValid(Closer);
        }
    }
}