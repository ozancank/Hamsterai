enum UserRole {
  admin('admin', 1),
  teacher('teacher', 2),
  student('student', 3);

  const UserRole(this.name, this.level);

  final String name;
  final int level;
  @override
  String toString() => name;

  factory UserRole.fromJson(String? role) {
    switch (role) {
      case "admin":
        return UserRole.admin;
      case "teacher":
        return UserRole.teacher;
      default:
        return UserRole.student;
    }
  }
}
