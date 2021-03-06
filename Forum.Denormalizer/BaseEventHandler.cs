﻿using ENode.Infrastructure;

namespace Forum.Denormalizer
{
    public class BaseEventHandler {

        protected ISqlQueryDbConnectionFactory ConnectionFactory { get; private set; }

        public BaseEventHandler(ISqlQueryDbConnectionFactory connectionFactory) {
            ConnectionFactory = connectionFactory;
        }
    }
}
