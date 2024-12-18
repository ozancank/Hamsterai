class ApplicationConstants {
  // ignore: constant_identifier_names
  static const testBaseUrl = "https://testapi.hamsterai.com.tr";
  static const APIBASEURL = "https://api.hamsterai.com.tr";
  //static const APIBASEURL = "http://192.168.1.51:5005";
  // ignore: constant_identifier_names
  static const XAPIKEY = {"X-Api-Key": "HaMsTerAI-Security"};
  //static const APIBASEURL = "http://192.168.1.101:5005";

  static String getBaseUrl(String email) {
    if (email.endsWith('@testmail.com')) {
      return testBaseUrl;
    }
    return APIBASEURL;
  }
}
