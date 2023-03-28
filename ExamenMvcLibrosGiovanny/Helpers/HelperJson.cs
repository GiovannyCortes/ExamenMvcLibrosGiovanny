﻿using Newtonsoft.Json;

namespace ExamenMvcLibrosGiovanny.Helpers {
    public static class HelperJson {

        public static T DeserializeObject<T>(string data) {
            T objeto = JsonConvert.DeserializeObject<T>(data);
            return objeto;
        }

        public static string SerializeObject<T>(T data) {
            string json = JsonConvert.SerializeObject(data);
            return json;
        }

    }
}
