import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:get/get.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/pages/common/common_appbar.dart';
import 'package:mobile/pages/common/common_button.dart';
import 'package:mobile/pages/profileDetail/view/common/profile_text_field.dart';
import 'package:mobile/pages/updatePassword/controller/update_password_controller.dart';
import 'package:mobile/styles/colors.dart';

class UpdatePasswordPage extends StatefulWidget {
  const UpdatePasswordPage({super.key});

  @override
  State<UpdatePasswordPage> createState() => _UpdatePasswordPageState();
}

class _UpdatePasswordPageState extends State<UpdatePasswordPage> {
  final updatePasswordController = Get.put(UpdatePasswordController());
  final formKey = GlobalKey<FormState>();
  final oldPasswordController = TextEditingController();
  final newPasswordController = TextEditingController();
  final againPasswordController = TextEditingController();
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: const CommonAppbar(title: 'Şifre Güncelle'),
      backgroundColor: Colors.white,
      body: SingleChildScrollView(
        child: Form(
          key: formKey,
          child: Column(
            children: [
              ProfileTextField(
                isObsecure: true,
                controller: oldPasswordController,
                hintText: 'Güncel Parola',
                validator: (value) {
                  if (value!.isEmpty) {
                    return;
                  }
                  return null;
                },
              ),
              ProfileTextField(
                isObsecure: true,
                controller: newPasswordController,
                hintText: 'Yeni Parola',
                validator: (value) {
                  if (value!.isEmpty) {
                    return;
                  }
                  return null;
                },
              ),
              ProfileTextField(
                isObsecure: true,
                controller: againPasswordController,
                hintText: 'Yeni Parola Tekrar',
                validator: (value) {
                  if (value!.isEmpty) {
                    return;
                  }
                  return null;
                },
              ),
              Padding(
                padding: EdgeInsets.symmetric(
                  horizontal: context.dynamicWidth * 0.05,
                ),
                child: CommonButton(
                  textColor: Colors.white,
                  onTap: () {
                    HapticFeedback.mediumImpact();
                    if (newPasswordController.text !=
                        againPasswordController.text) {
                      return;
                    }
                    if (!formKey.currentState!.validate()) {
                      return;
                    }
                    updatePasswordController.updatePassword(
                        oldPasswordController.text,
                        newPasswordController.text,
                        context);
                  },
                  bgcolor: MyColors.primaryColor,
                  text: 'Güncelle',
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
