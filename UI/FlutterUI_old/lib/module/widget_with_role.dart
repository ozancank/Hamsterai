import 'package:flutter/material.dart';
import 'package:mobile/core/enums/locale_keys_enum.dart';
import 'package:mobile/core/init/cache/local_manager.dart';
import 'package:mobile/pages/auth/model/login_response_model.dart';

class WidgetWithRole extends StatelessWidget {
  const WidgetWithRole({super.key, required this.child});

  final Widget child;

  Future<LoginResponseModel?> getUser() async {
    return await LocaleManager.instance.getUserModelValue(PreferencesKeys.USER);
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<LoginResponseModel?>(
      future: getUser(),
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return const Center(child: CircularProgressIndicator());
        }
        if (snapshot.hasError) {
          return Center(child: Text('Bir hata oluştu: ${snapshot.error}'));
        }
        if (!snapshot.hasData) {
          return Container();
        }

        final model = snapshot.data!;
        final user = model.userInfo;

        if (user.type == 1) {
          return child;
        }

        return Container();
      },
    );
  }
}
