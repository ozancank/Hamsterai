import 'package:dio/dio.dart';
import 'package:mobile/core/base/base_service.dart';
import 'package:mobile/core/enums/service_endpoints.dart';
import 'package:mobile/pages/auth/model/error_model.dart';
import 'package:mobile/pages/homeworks/model/homework_model.dart';

class HomeworkService extends BaseService {
  HomeworkService(super.dio);

  Future getHomeworks(int lessonId) async {
    getAuth();
    try {
      final response = await dio.post(
        '${ServiceEndpoints.getHomeworks.getEndpoint}?Page=0&PageSize=0',
        data: {'lessonId': lessonId},
      );
      if (response.statusCode == 200) {
        final data = HomeWorkModel.fromJson(response.data);
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
