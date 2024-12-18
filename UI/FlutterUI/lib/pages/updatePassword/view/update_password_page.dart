import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:get/get.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/pages/common/common_appbar.dart';
import 'package:mobile/pages/common/common_appbar2.dart';
import 'package:mobile/pages/common/common_button.dart';
import 'package:mobile/pages/profileDetail/view/common/profile_text_field.dart';
import 'package:mobile/pages/updatePassword/controller/update_password_controller.dart';
import 'package:mobile/styles/colors.dart';
import 'package:quickalert/models/quickalert_type.dart';
import 'package:quickalert/widgets/quickalert_dialog.dart';

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
  bool isObsecureOld = true;
  bool isObsecureNew = true;
  bool isObsecureAgain = true;
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CommonAppbar2(title: 'Şifre Güncelle'),
      backgroundColor: Colors.white,
      body: GestureDetector(
        onTap: () => FocusScope.of(context).unfocus(),
        child: SingleChildScrollView(
          child: Form(
            key: formKey,
            child: Column(
              children: [
                ProfileTextField(
                  isObsecure: isObsecureOld,
                  controller: oldPasswordController,
                  hintText: 'Güncel Parola',
                  isPassword: true,
                  validator: (value) {
                    if (value!.isEmpty) {
                      return;
                    }
                    return null;
                  },
                  onTap: () {
                    HapticFeedback.mediumImpact();
                    setState(() {
                      isObsecureOld = !isObsecureOld;
                    });
                  },
                ),
                ProfileTextField(
                  isObsecure: isObsecureNew,
                  controller: newPasswordController,
                  hintText: 'Yeni Parola',
                  isPassword: true,
                  validator: (value) {
                    if (value!.isEmpty) {
                      return;
                    }
                    return null;
                  },
                  onTap: () {
                    HapticFeedback.mediumImpact();
                    setState(() {
                      isObsecureNew = !isObsecureNew;
                    });
                  },
                ),
                ProfileTextField(
                  isObsecure: isObsecureAgain,
                  controller: againPasswordController,
                  hintText: 'Yeni Parola Tekrar',
                  isPassword: true,
                  validator: (value) {
                    if (value!.isEmpty) {
                      return;
                    }
                    return null;
                  },
                  onTap: () {
                    HapticFeedback.mediumImpact();
                    setState(() {
                      isObsecureAgain = !isObsecureAgain;
                    });
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
                        QuickAlert.show(
                          context: context,
                          type: QuickAlertType.error,
                          text: 'Şifreler uyuşmuyor',
                          confirmBtnText: 'Tamam',
                          confirmBtnColor: MyColors.redColor,
                          confirmBtnTextStyle: Theme.of(context)
                              .textTheme
                              .bodyMedium!
                              .copyWith(color: Colors.white),
                          title: 'Hata',
                        );
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
      ),
    );
  }
}
