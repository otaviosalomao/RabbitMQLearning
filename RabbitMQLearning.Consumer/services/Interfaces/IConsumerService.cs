﻿using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQLearning.Consumer.services.Interfaces
{
    public interface IConsumerService
    {
        Task Process();
    }
}
