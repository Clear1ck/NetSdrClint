using NUnit.Framework;
using NetSdrClientApp.Messages;
using System;

namespace NetSdrClientAppTests
{
    public class NetSdrMessageHelperTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetControlItemMessageTest()
        {
            //Arrange
            var type = NetSdrMessageHelper.MsgTypes.Ack;
            var code = NetSdrMessageHelper.ControlItemCodes.ReceiverState;
            int parametersLength = 7500;

            //Act
            byte[] msg = NetSdrMessageHelper.GetControlItemMessage(type, code, new byte[parametersLength]);

            var headerBytes = msg.Take(2);
            var codeBytes = msg.Skip(2).Take(2);
            var parametersBytes = msg.Skip(4);

            var num = BitConverter.ToUInt16(headerBytes.ToArray());
            var actualType = (NetSdrMessageHelper.MsgTypes)(num >> 13);
            var actualLength = num - ((int)actualType << 13);
            var actualCode = BitConverter.ToInt16(codeBytes.ToArray());

            //Assert
            Assert.That(headerBytes.Count(), Is.EqualTo(2));
            Assert.That(msg.Length, Is.EqualTo(actualLength));
            Assert.That(type, Is.EqualTo(actualType));

            Assert.That(actualCode, Is.EqualTo((short)code));

            Assert.That(parametersBytes.Count(), Is.EqualTo(parametersLength));
        }

        [Test]
        public void GetDataItemMessageTest()
        {
            //Arrange
            var type = NetSdrMessageHelper.MsgTypes.DataItem2;
            int parametersLength = 7500;

            //Act
            byte[] msg = NetSdrMessageHelper.GetDataItemMessage(type, new byte[parametersLength]);

            var headerBytes = msg.Take(2);
            var parametersBytes = msg.Skip(2);

            var num = BitConverter.ToUInt16(headerBytes.ToArray());
            var actualType = (NetSdrMessageHelper.MsgTypes)(num >> 13);
            var actualLength = num - ((int)actualType << 13);

            //Assert
            Assert.That(headerBytes.Count(), Is.EqualTo(2));
            Assert.That(msg.Length, Is.EqualTo(actualLength));
            Assert.That(type, Is.EqualTo(actualType));

            Assert.That(parametersBytes.Count(), Is.EqualTo(parametersLength));
        }

        [Test]
        public void TranslateMessage_EmptyData_ShouldNotThrow()
        {
            // Arrange
            byte[] data = Array.Empty<byte>();

            // Act & Assert
            Assert.DoesNotThrow(() => 
                NetSdrMessageHelper.TranslateMessage(data, out _, out _, out _, out _)
            );
        }

        [Test]
        public void TranslateMessage_TooShortData_ShouldReturnDefault()
        {
            // Arrange
            byte[] data = new byte[] { 0x01, 0x02 };

            // Act
            NetSdrMessageHelper.TranslateMessage(data, out var type, out _, out _, out _);

            // Assert
            Assert.That((int)type, Is.EqualTo(0));
        }

        [Test]
        public void TranslateMessage_ValidData_ShouldParseType()
        {
            // Arrange
            ushort combined = (ushort)((int)NetSdrMessageHelper.MsgTypes.DataItem1 << 13);
            byte[] data = BitConverter.GetBytes(combined); 
            
            var fullData = data.Concat(new byte[] { 0x00, 0x00 }).ToArray();

            // Act
            NetSdrMessageHelper.TranslateMessage(fullData, out var actualType, out _, out _, out _);

            // Assert
            Assert.That(actualType, Is.EqualTo(NetSdrMessageHelper.MsgTypes.DataItem1));
        }

        [Test]
        public void TranslateMessage_WithBody_ShouldReturnBodyBytes()
        {
            // Arrange
            byte[] data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0xAA, 0xBB };

            // Act
            NetSdrMessageHelper.TranslateMessage(data, out _, out _, out _, out byte[] body);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(body, Has.Length.EqualTo(2));
                Assert.That(body[0], Is.EqualTo(0xAA));
                Assert.That(body[1], Is.EqualTo(0xBB));
            });
        }
        [Test]
        public void TranslateMessage_NullInput_ShouldReturnFalse()
        {
            // Act
            bool result = NetSdrMessageHelper.TranslateMessage(null!, out _, out _, out _, out _);

            // Assert
            Assert.That(result, Is.False); 

        }

        [Test]
        public void TranslateMessage_EmptyArray_ShouldReturnFalse()
        {
            // Act
            bool result = NetSdrMessageHelper.TranslateMessage(Array.Empty<byte>(), out _, out _, out _, out _);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TranslateMessage_TooShortArray_ShouldReturnFalse()
        {
            // Arrange
            byte[] data = new byte[] { 0x01, 0x02, 0x03 }; // Менше 4 байт

            // Act
            bool result = NetSdrMessageHelper.TranslateMessage(data, out _, out _, out _, out _);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}
