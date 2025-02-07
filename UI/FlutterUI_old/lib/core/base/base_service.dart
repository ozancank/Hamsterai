import 'package:dio/dio.dart';
import 'package:mobile/core/enums/locale_keys_enum.dart';
import 'package:mobile/core/init/cache/local_manager.dart';

class BaseService {
  final Dio dio;
  BaseService(this.dio);

  void getAuth() {
    final String personalInfoToken =
        LocaleManager.instance.getStringValue(PreferencesKeys.TOKEN);
    try {
      personalInfoToken != ''
          ? dio.options.headers["Authorization"] = "Bearer $personalInfoToken"
          : null;
      dio.options.headers["X-Api-Key"] = "HaMsTerAI-Security";
    } catch (e) {
      throw Exception('Some auth error');
    }
  }
}
