using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IotHub {

    static string descriptor = "" +
      "{\"systems\":{" +
      "\"name\":\"Buttons\"," +
      "\"count\":1," +
      "\"sys1\":{" +
        "\"id\":1," +
        "\"ui\":{" +
          "\"type\":\"toggle\"," +
          "\"actions\":{" +
            "\"count\":2," +
            "\"action1\":{\"event\":\"toggleon\",\"method\":\"1_on\"}," +
            "\"action2\":{\"event\":\"toggleoff\",\"method\":\"1_off\"}" +
          "}" +
        "}}," +
      //"\"sys2\":{" +
      //  "\"id\":2," +
      //  "\"ui\":{" +
      //    "\"type\":\"toggle\"," +
      //    "\"actions\":{" +
      //      "\"count\":2," +
      //      "\"action1\":{\"event\":\"toggleon\",\"method\":\"2_on\"}," +
      //      "\"action2\":{\"event\":\"toggleoff\",\"method\":\"2_off\"}" +
      //    "}" +
      //  "}}" +
      "}}";


    public static DeviceModel GetDeviceModelForCode(string code)
    {
        var parser = new DeviceParser(descriptor);
        return parser.Device;
    }

    public static void SendMethod(string method)
    {
        Debug.Log("sending method: " + method);
    }

}
