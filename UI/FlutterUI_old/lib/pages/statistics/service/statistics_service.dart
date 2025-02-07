import 'package:dio/dio.dart';
import 'package:mobile/core/base/base_service.dart';
import 'package:mobile/core/enums/service_endpoints.dart';
import 'package:mobile/pages/auth/model/error_model.dart';
import 'package:mobile/pages/statistics/model/gain_model.dart';

class StatisticsService extends BaseService {
  StatisticsService(super.dio);

  Future getStudentGain() async {
    getAuth();
    try {
      final response = await dio
          .get(ServiceEndpoints.getStudentGains.getEndpoint);
      if (response.statusCode == 200) {
        final data = LessonDataModel.fromJson(response.data);
        return data;
      }
    } on DioException catch (e) {
      final errorJson = e.response?.data;
      return ErrorModel.fromJson(errorJson);
    } catch (e) {
      return ErrorModel(statusCode: 500, message: 'Bilinmeyen bir hata oluştu');
    }
    return ErrorModel(statusCode: 400, message: 'Hata');
  }
}
