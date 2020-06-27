using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events
{
    public interface IEventStoreService
    {
        public void PublishEvent(string metaData, string data, string eventType,string stream);

        public List<Object> ReadEvents(string projectionName);

        public List<CartItem> ReadCartEvents(string projectionName);

        public void CreateProjection(string name, string eventType);

    }
}
