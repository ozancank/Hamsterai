import 'dart:convert';

UpdateUserResponseModel updateUserResponseModelFromJson(String str) =>
    UpdateUserResponseModel.fromJson(json.decode(str));

String updateUserResponseModelToJson(UpdateUserResponseModel data) =>
    json.encode(data.toJson());

class UpdateUserResponseModel {
  int id;
  bool? isActive;
  String? userName;
  String? name;
  String? surname;
  String? phone;
  String? profileFileName;
  String? email;
  String? fullName;
  int? connectionId;
  int? schoolId;
  int? type;

  UpdateUserResponseModel({
    required this.id,
    this.isActive,
    this.userName,
    this.name,
    this.surname,
    this.phone,
    this.profileFileName,
    this.email,
    this.fullName,
    this.connectionId,
    this.schoolId,
    this.type,
  });

  factory UpdateUserResponseModel.fromJson(Map<String, dynamic> json) =>
      UpdateUserResponseModel(
        id: json["id"],
        isActive: json["isActive"],
        userName: json["userName"],
        name: json["name"],
        surname: json["surname"],
        phone: json["phone"],
        profileFileName: json["profileFileName"],
        email: json["email"],
        fullName: json["fullName"],
        connectionId: json["connectionId"],
        schoolId: json["schoolId"],
        type: json["type"],
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "isActive": isActive,
        "userName": userName,
        "name": name,
        "surname": surname,
        "phone": phone,
        "profileFileName": profileFileName,
        "email": email,
        "fullName": fullName,
        "connectionId": connectionId,
        "schoolId": schoolId,
        "type": type,
      };
}
