using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace TurnCardGame.Serialization
{
    public static class XmlDataLoader<T> where T : struct
    {
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(T));

        public static T Deserialize(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
                throw new ArgumentException("XML content cannot be null or empty.", nameof(xml));

            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                IgnoreComments = true,
                IgnoreWhitespace = true
            };

            using (var stringReader = new StringReader(xml))
            using (var xmlReader = XmlReader.Create(stringReader, settings))
            {
                object result = Serializer.Deserialize(xmlReader);
                if (result is T data)
                    return data;
            }

            throw new InvalidDataException($"XML could not be deserialized as {typeof(T).FullName}.");
        }

        public static T Deserialize(TextAsset asset)
        {
            if (asset == null)
                throw new ArgumentNullException(nameof(asset));

            return Deserialize(asset.text);
        }

        public static bool TryDeserialize(string xml, out T data, out string error)
        {
            try
            {
                data = Deserialize(xml);
                error = string.Empty;
                return true;
            }
            catch (Exception exception) when (
                exception is ArgumentException ||
                exception is InvalidOperationException ||
                exception is InvalidDataException ||
                exception is XmlException)
            {
                data = default;
                error = exception.Message;
                return false;
            }
        }

        public static bool TryDeserialize(TextAsset asset, out T data, out string error)
        {
            if (asset == null)
            {
                data = default;
                error = "TextAsset cannot be null.";
                return false;
            }

            return TryDeserialize(asset.text, out data, out error);
        }

        public static T LoadFromResources(string resourcePath)
        {
            if (string.IsNullOrWhiteSpace(resourcePath))
                throw new ArgumentException("Resource path cannot be null or empty.", nameof(resourcePath));

            TextAsset asset = Resources.Load<TextAsset>(resourcePath);
            if (asset == null)
                throw new FileNotFoundException($"XML TextAsset was not found at Resources/{resourcePath}.");

            return Deserialize(asset);
        }

        public static bool TryLoadFromResources(string resourcePath, out T data, out string error)
        {
            try
            {
                data = LoadFromResources(resourcePath);
                error = string.Empty;
                return true;
            }
            catch (Exception exception) when (
                exception is ArgumentException ||
                exception is FileNotFoundException ||
                exception is InvalidOperationException ||
                exception is InvalidDataException ||
                exception is XmlException)
            {
                data = default;
                error = exception.Message;
                return false;
            }
        }
    }
}
