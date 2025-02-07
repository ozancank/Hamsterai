import 'dart:convert';

UpdateUserPostModel updateUserPostModelFromJson(String str) =>
    UpdateUserPostModel.fromJson(json.decode(str));

String updateUserPostModelToJson(UpdateUserPostModel data) =>
    json.encode(data.toJson());

class UpdateUserPostModel {
  int id;
  String userName;
  String name;
  String surname;
  String phone;
  String profileUrl;
  String email;
  int type;
  int connectionId;
  int schoolId;
  String profilePictureBase64;
  String profilePictureFileName;

  UpdateUserPostModel({
    required this.id,
    required this.userName,
    required this.name,
    required this.surname,
    required this.phone,
    required this.profileUrl,
    required this.email,
    required this.type,
    required this.connectionId,
    required this.schoolId,
    required this.profilePictureBase64,
    required this.profilePictureFileName,
  });

  factory UpdateUserPostModel.fromJson(Map<String, dynamic> json) =>
      UpdateUserPostModel(
        id: json["id"],
        userName: json["userName"],
        name: json["name"],
        surname: json["surname"],
        phone: json["phone"],
        profileUrl: json["profileUrl"],
        email: json["email"],
        type: json["type"],
        connectionId: json["connectionId"],
        schoolId: json["schoolId"],
        profilePictureBase64: json["profilePictureBase64"],
        profilePictureFileName: json["profilePictureFileName"],
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "userName": userName,
        "name": name,
        "surname": surname,
        "phone": phone,
        "profileUrl": profileUrl,
        "email": email,
        "type": type,
        "connectionId": connectionId,
        "schoolId": schoolId,
        "profilePictureBase64": profilePictureBase64,
        "profilePictureFileName": profilePictureFileName,
      };
}
