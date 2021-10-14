using Azure;
using Azure.Data.Tables;
using RundooApi.Entities;
using RundooApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RundooApi.Services
{
    public class TablesService
    {

        public TablesService(TableClient tableClient)
        {
            _tableClient = tableClient;
        }

        private TableClient _tableClient;

        public string[] EXCLUDE_TABLE_ENTITY_KEYS = { "PartitionKey", "RowKey", "odata.etag", "Timestamp" };



        public IEnumerable<WeatherDataModel> GetAllRows()
        {
            Pageable<TableEntity> entities = _tableClient.Query<TableEntity>();

            return entities.Select(e => MapTableEntityToWeatherDataModel(e));
        }


        public IEnumerable<WeatherDataModel> GetFilteredRows(FilterResultsInputModel inputModel)
        {
            List<string> filters = new List<string>();

            if (!String.IsNullOrEmpty(inputModel.PartitionKey))
                filters.Add($"PartitionKey eq '{inputModel.PartitionKey}'");
            if (!String.IsNullOrEmpty(inputModel.RowKeyDateStart) && !String.IsNullOrEmpty(inputModel.RowKeyTimeStart))
                filters.Add($"RowKey ge '{inputModel.RowKeyDateStart} {inputModel.RowKeyTimeStart}'");
            if (!String.IsNullOrEmpty(inputModel.RowKeyDateEnd) && !String.IsNullOrEmpty(inputModel.RowKeyTimeEnd))
                filters.Add($"RowKey le '{inputModel.RowKeyDateEnd} {inputModel.RowKeyTimeEnd}'");
            if (inputModel.MinTemperature.HasValue)
                filters.Add($"Temperature ge {inputModel.MinTemperature.Value}");
            if (inputModel.MaxTemperature.HasValue)
                filters.Add($"Temperature le {inputModel.MaxTemperature.Value}");
            if (inputModel.MinPrecipitation.HasValue)
                filters.Add($"Precipitation ge {inputModel.MinTemperature.Value}");
            if (inputModel.MaxPrecipitation.HasValue)
                filters.Add($"Precipitation le {inputModel.MaxTemperature.Value}");

            string filter = String.Join(" and ", filters);
            Pageable<TableEntity> entities = _tableClient.Query<TableEntity>(filter);

            return entities.Select(e => MapTableEntityToWeatherDataModel(e));
        }


        public WeatherDataModel MapTableEntityToWeatherDataModel(TableEntity entity)
        {
            WeatherDataModel observation = new WeatherDataModel();
            observation.StationName = entity.PartitionKey;
            observation.ObservationDate = entity.RowKey;
            observation.Timestamp = entity.Timestamp;
            observation.Etag = entity.ETag.ToString();

            var measurements = entity.Keys.Where(key => !EXCLUDE_TABLE_ENTITY_KEYS.Contains(key));
            foreach (var key in measurements)
            {
                observation[key] = entity[key];
            }
            return observation;            
        }



        public void InsertTableEntity(WeatherInputModel model)
        {
            TableEntity entity = new TableEntity();
            entity.PartitionKey = model.StationName;
            entity.RowKey = $"{model.ObservationDate} {model.ObservationTime}";

            // The other values are added like a items to a dictionary
            entity["Temperature"] = model.Temperature;
            entity["Humidity"] = model.Humidity;
            entity["Barometer"] = model.Barometer;
            entity["WindDirection"] = model.WindDirection;
            entity["WindSpeed"] = model.WindSpeed;
            entity["Precipitation"] = model.Precipitation;

            _tableClient.AddEntity(entity);
        }


        public void UpsertTableEntity(WeatherInputModel model)
        {
            TableEntity entity = new TableEntity();
            entity.PartitionKey = model.StationName;
            entity.RowKey = $"{model.ObservationDate} {model.ObservationTime}";

            // The other values are added like a items to a dictionary
            entity["Temperature"] = model.Temperature;
            entity["Humidity"] = model.Humidity;
            entity["Barometer"] = model.Barometer;
            entity["WindDirection"] = model.WindDirection;
            entity["WindSpeed"] = model.WindSpeed;
            entity["Precipitation"] = model.Precipitation;

            _tableClient.UpsertEntity(entity);
        }


        public void InsertExpandableData(ExpandableWeatherObject weatherObject)
        {
            TableEntity entity = new TableEntity();
            entity.PartitionKey = weatherObject.StationName;
            entity.RowKey = weatherObject.ObservationDate;

            foreach (string propertyName in weatherObject.PropertyNames)
            {
                var value = weatherObject[propertyName];
                entity[propertyName] = value;
            }
            _tableClient.AddEntity(entity);
        }

        
        public void UpsertExpandableData(ExpandableWeatherObject weatherObject)
        {
            TableEntity entity = new TableEntity();
            entity.PartitionKey = weatherObject.StationName;
            entity.RowKey = weatherObject.ObservationDate;

            foreach (string propertyName in weatherObject.PropertyNames)
            {
                var value = weatherObject[propertyName];
                entity[propertyName] = value;
            }
            _tableClient.UpsertEntity(entity);
        }

        public void RemoveEntity(string partitionKey, string rowKey)
        {
            _tableClient.DeleteEntity(partitionKey, rowKey);           
        }


        public void UpdateEntity(UpdateWeatherObject weatherObject)
        {
            string partitionKey = weatherObject.StationName;
            string rowKey = weatherObject.ObservationDate;

            // Use the partition key and row key to get the entity
            TableEntity entity = _tableClient.GetEntity<TableEntity>(partitionKey, rowKey).Value;

            foreach (string propertyName in weatherObject.PropertyNames)
            {
                var value = weatherObject[propertyName];
                entity[propertyName] = value;
            }

            _tableClient.UpdateEntity(entity, new ETag(weatherObject.Etag));
        }

    }
}
