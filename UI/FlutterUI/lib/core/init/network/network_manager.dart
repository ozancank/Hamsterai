import 'package:dio/dio.dart';
import 'package:mobile/core/constants/app_constant.dart';
import 'package:mobile/core/init/cache/url_storage.dart';
import 'package:pretty_dio_logger/pretty_dio_logger.dart';

class NetworkManager {
  static NetworkManager? _instance;

  static NetworkManager get instance {
    if (_instance != null) return _instance!;

    _instance = NetworkManager._init(ApplicationConstants.APIBASEURL);
    return _instance!;
  }

  static Future<NetworkManager> getInstance() async {
    if (_instance != null) return _instance!;

    String? savedBaseUrl = await UrlStorage.getBaseUrl();
    savedBaseUrl ??= ApplicationConstants.APIBASEURL;

    _instance = NetworkManager._init(savedBaseUrl);
    return _instance!;
  }

  late final Dio dio;
  late String _baseUrl;
  NetworkManager._init(String baseUrl) {
    _baseUrl = baseUrl;
    dio = Dio(BaseOptions(baseUrl: _baseUrl));
    dio.interceptors.add(PrettyDioLogger());
  }

  void setBaseUrl(String baseUrl) async {
    _baseUrl = baseUrl;
    dio.options.baseUrl = baseUrl;
    await UrlStorage.saveBaseUrl(baseUrl);
  }
}
