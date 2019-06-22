﻿// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Steeltoe.Messaging;
using Steeltoe.Messaging.Support;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Steeltoe.Integration.Support.Test
{
    public class MutableMessageBuilderTest
    {
        [Fact]
        public void Mutable()
        {
            MutableMessageBuilder<string> builder = MutableMessageBuilder<string>.WithPayload("test");
            IMessage<string> message1 = builder.SetHeader("foo", "bar").Build();
            IMessage<string> message2 = MutableMessageBuilder<string>.FromMessage(message1).SetHeader("another", 1).Build();
            Assert.Equal("bar", message2.Headers["foo"]);
            Assert.Equal(message1.Headers.Id, message2.Headers.Id);
            Assert.True(message2 == message1);
        }

        [Fact]
        public void MutableFromImmutable()
        {
            IMessage<string> message1 = MessageBuilder<string>.WithPayload("test").SetHeader("foo", "bar").Build();
            IMessage<string> message2 = MutableMessageBuilder<string>.FromMessage(message1).SetHeader("another", 1).Build();
            Assert.Equal("bar", message2.Headers["foo"]);
            Assert.Equal(message1.Headers.Id, message2.Headers.Id);
            Assert.NotEqual(message1, message2);
            Assert.False(message2 == message1);
        }

        [Fact]
        public void MutableFromImmutableMutate()
        {
            IMessage<string> message1 = MessageBuilder<string>.WithPayload("test").SetHeader("foo", "bar").Build();
            IMessage<string> message2 = new MutableMessageBuilderFactory().FromMessage(message1).SetHeader("another", 1).Build();
            Assert.Equal("bar", message2.Headers["foo"]);
            Assert.Equal(message1.Headers.Id, message2.Headers.Id);
            Assert.NotEqual(message1, message2);
            Assert.False(message2 == message1);
        }

        [Fact]
        public void TestPushAndPopSequenceDetailsMutable()
        {
            IMessage<int> message1 = MutableMessageBuilder<int>.WithPayload(1).PushSequenceDetails("foo", 1, 2).Build();
            Assert.False(message1.Headers.ContainsKey(IntegrationMessageHeaderAccessor.SEQUENCE_DETAILS));
            IMessage<int> message2 = MutableMessageBuilder<int>.FromMessage(message1).PushSequenceDetails("bar", 1, 1).Build();
            Assert.True(message2.Headers.ContainsKey(IntegrationMessageHeaderAccessor.SEQUENCE_DETAILS));
            IMessage<int> message3 = MutableMessageBuilder<int>.FromMessage(message2).PopSequenceDetails().Build();
            Assert.False(message3.Headers.ContainsKey(IntegrationMessageHeaderAccessor.SEQUENCE_DETAILS));
        }

        [Fact]
        public void TestPushAndPopSequenceDetailsWhenNoCorrelationIdMutable()
        {
            IMessage<int> message1 = MutableMessageBuilder<int>.WithPayload(1).Build();
            Assert.False(message1.Headers.ContainsKey(IntegrationMessageHeaderAccessor.SEQUENCE_DETAILS));
            IMessage<int> message2 = MutableMessageBuilder<int>.FromMessage(message1).PushSequenceDetails("bar", 1, 1).Build();
            Assert.False(message2.Headers.ContainsKey(IntegrationMessageHeaderAccessor.SEQUENCE_DETAILS));
            IMessage<int> message3 = MutableMessageBuilder<int>.FromMessage(message2).PopSequenceDetails().Build();
            Assert.False(message3.Headers.ContainsKey(IntegrationMessageHeaderAccessor.SEQUENCE_DETAILS));
        }

        [Fact]
        public void TestPopSequenceDetailsWhenNotPoppedMutable()
        {
            IMessage<int> message1 = MutableMessageBuilder<int>.WithPayload(1).Build();
            Assert.False(message1.Headers.ContainsKey(IntegrationMessageHeaderAccessor.SEQUENCE_DETAILS));
            IMessage<int> message2 = MutableMessageBuilder<int>.FromMessage(message1).PopSequenceDetails().Build();
            Assert.False(message2.Headers.ContainsKey(IntegrationMessageHeaderAccessor.SEQUENCE_DETAILS));
        }

        [Fact]
        public void TestPushAndPopSequenceDetailsWhenNoSequenceMutable()
        {
            IMessage<int> message1 = MutableMessageBuilder<int>.WithPayload(1).SetCorrelationId("foo").Build();
            Assert.False(message1.Headers.ContainsKey(IntegrationMessageHeaderAccessor.SEQUENCE_DETAILS));
            IMessage<int> message2 = MutableMessageBuilder<int>.FromMessage(message1).PushSequenceDetails("bar", 1, 1).Build();
            Assert.True(message2.Headers.ContainsKey(IntegrationMessageHeaderAccessor.SEQUENCE_DETAILS));
            IMessage<int> message3 = MutableMessageBuilder<int>.FromMessage(message2).PopSequenceDetails().Build();
            Assert.False(message3.Headers.ContainsKey(IntegrationMessageHeaderAccessor.SEQUENCE_DETAILS));
        }

        [Fact]
        public void TestNoIdAndTimestampHeaders()
        {
            IMessage<string> message =
                    MutableMessageBuilder<string>.WithPayload("foo", false)
                            .PushSequenceDetails("bar", 1, 1)
                            .Build();
            Assert.True(message.Headers.ContainsKey(IntegrationMessageHeaderAccessor.CORRELATION_ID));
            Assert.True(message.Headers.ContainsKey(IntegrationMessageHeaderAccessor.SEQUENCE_NUMBER));
            Assert.True(message.Headers.ContainsKey(IntegrationMessageHeaderAccessor.SEQUENCE_SIZE));
            Assert.False(message.Headers.ContainsKey(IntegrationMessageHeaderAccessor.SEQUENCE_DETAILS));
            Assert.False(message.Headers.ContainsKey(MessageHeaders.ID));
            Assert.False(message.Headers.ContainsKey(MessageHeaders.TIMESTAMP));
        }
    }
}
