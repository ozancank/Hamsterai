import 'dart:io';

import 'package:dio/dio.dart';
import 'package:mobile/core/base/base_service.dart';
import 'package:mobile/core/enums/service_endpoints.dart';
import 'package:mobile/pages/auth/model/error_model.dart';
import 'package:mobile/pages/auth/model/login_response_model.dart';
import 'package:mobile/pages/auth/model/student_response_model.dart';
import 'package:mobile/pages/profileDetail/model/update_user_post_model.dart';
import 'package:mobile/pages/profileDetail/model/update_user_response_model.dart';

class UpdateUserService extends BaseService {
  UpdateUserService(super.dio);

  Future updateUser(UpdateUserPostModel updateModel) async {
    getAuth();
    try {
      final response = await dio.post(ServiceEndpoints.updateUser.getEndpoint,
          data: updateModel.toJson());

      if (response.statusCode == HttpStatus.ok) {
        final data = UserInfo.fromJson(response.data);
        return data;
      }
    } on DioException catch (e) {
      final errorJson = e.response?.data;

      return ErrorModel.fromJson(errorJson);
    } catch (e) {
      return ErrorModel(statusCode: 500, message: 'Bilinmeyen bir hata oluştu');
    }
  }

  Future updateUserPassword(String id, String password) async {
    getAuth();
    try {
      final response =
          await dio.post(ServiceEndpoints.updatePassword.getEndpoint, data: {
        'id': id,
        'password': password,
      });
      if (response.statusCode == HttpStatus.ok) {
        final data = UpdateUserResponseModel.fromJson(response.data);
        return data;
      }
    } on DioException catch (e) {
      final errorJson = e.response?.data;

      return ErrorModel.fromJson(errorJson);
    } catch (e) {
      return ErrorModel(statusCode: 500, message: 'Bilinmeyen bir hata oluştu');
    }
  }

  Future getStudentById(int userId) async {
    getAuth();
    try {
      final response = await dio
          .get('${ServiceEndpoints.getStudentById.getEndpoint}/$userId');
      if (response.statusCode == HttpStatus.ok) {
        final data = StudentResponseModel.fromJson(response.data);
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
