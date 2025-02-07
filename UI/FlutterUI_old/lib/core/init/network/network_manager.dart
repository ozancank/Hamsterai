import 'package:dio/dio.dart';
import 'package:mobile/core/constants/app_constant.dart';
// ignore: depend_on_referenced_packages
import 'package:pretty_dio_logger/pretty_dio_logger.dart';

class NetworkManager {
  static NetworkManager? _instance;

  static NetworkManager get instance {
    if (_instance != null) return _instance!;

    _instance = NetworkManager._init();
    return _instance!;
  }

  final String _baseUrl = ApplicationConstants.APIBASEURL;
  late final Dio dio;

  NetworkManager._init() {
    dio = Dio(BaseOptions(baseUrl: _baseUrl));
    dio.interceptors.add(PrettyDioLogger());
  }
}
