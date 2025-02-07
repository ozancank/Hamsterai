import 'dart:convert';

StudentResponseModel studentResponseModelFromJson(String str) =>
    StudentResponseModel.fromJson(json.decode(str));

String studentResponseModelToJson(StudentResponseModel data) =>
    json.encode(data.toJson());

class StudentResponseModel {
  int id;
  bool isActive;
  int createUser;
  DateTime createDate;
  int updateUser;
  DateTime updateDate;
  String name;
  String surname;
  String studentNo;
  String tcNo;
  String phone;
  String email;
  String classRoomId;
  String fullName;
  String className;
  List<String> teacherNames;

  StudentResponseModel({
    required this.id,
    required this.isActive,
    required this.createUser,
    required this.createDate,
    required this.updateUser,
    required this.updateDate,
    required this.name,
    required this.surname,
    required this.studentNo,
    required this.tcNo,
    required this.phone,
    required this.email,
    required this.classRoomId,
    required this.fullName,
    required this.className,
    required this.teacherNames,
  });

  factory StudentResponseModel.fromJson(Map<String, dynamic> json) =>
      StudentResponseModel(
        id: json["id"],
        isActive: json["isActive"],
        createUser: json["createUser"],
        createDate: DateTime.parse(json["createDate"]),
        updateUser: json["updateUser"],
        updateDate: DateTime.parse(json["updateDate"]),
        name: json["name"],
        surname: json["surname"],
        studentNo: json["studentNo"],
        tcNo: json["tcNo"],
        phone: json["phone"],
        email: json["email"],
        classRoomId: json["classRoomId"],
        fullName: json["fullName"],
        className: json["className"],
        teacherNames: List<String>.from(json["teacherNames"].map((x) => x)),
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "isActive": isActive,
        "createUser": createUser,
        "createDate": createDate.toIso8601String(),
        "updateUser": updateUser,
        "updateDate": updateDate.toIso8601String(),
        "name": name,
        "surname": surname,
        "studentNo": studentNo,
        "tcNo": tcNo,
        "phone": phone,
        "email": email,
        "classRoomId": classRoomId,
        "fullName": fullName,
        "className": className,
        "teacherNames": List<dynamic>.from(teacherNames.map((x) => x)),
      };
}
