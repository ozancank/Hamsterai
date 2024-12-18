import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:get/get.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/pages/common/common_button.dart';
import 'package:mobile/pages/updatePassword/controller/update_password_controller.dart';
import 'package:mobile/styles/colors.dart';
import 'package:quickalert/models/quickalert_type.dart';
import 'package:quickalert/widgets/quickalert_dialog.dart';

class UpdatePasswordForceView extends StatefulWidget {
  const UpdatePasswordForceView({super.key});

  @override
  State<UpdatePasswordForceView> createState() => _UpdatePasswordViewState();
}

class _UpdatePasswordViewState extends State<UpdatePasswordForceView> {
  final updatePasswordController = Get.put(UpdatePasswordController());
  final formKey = GlobalKey<FormState>();
  final oldPasswordController = TextEditingController();
  final newPasswordController = TextEditingController();
  final againPasswordController = TextEditingController();
  bool isObsecureOld = true;
  bool isObsecureNew = true;
  bool isObsecureAgain = true;

  final FocusNode oldPasswordFocusNode = FocusNode();
  final FocusNode newPasswordFocusNode = FocusNode();
  final FocusNode againPasswordFocusNode = FocusNode();

  @override
  void dispose() {
    oldPasswordFocusNode.dispose();
    newPasswordFocusNode.dispose();
    againPasswordFocusNode.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      resizeToAvoidBottomInset: true,
      body: GestureDetector(
        onTap: () {
          FocusScope.of(context).requestFocus(FocusNode());
        },
        child: SingleChildScrollView(
          child: Column(
            children: [
              SizedBox(height: MediaQuery.of(context).size.height * 0.03),
              Container(
                padding: EdgeInsets.all(context.dynamicHeight * 0.00),
                height: context.dynamicHeight * 0.35,
                width: double.infinity,
                alignment: Alignment.center,
                child: Image.asset(AssetsConstant.logo3),
              ),
              SizedBox(
                height: context.dynamicHeight * 0.01,
              ),
              Container(
                height: MediaQuery.of(context).size.height,
                decoration: BoxDecoration(
                  color: MyColors.primaryColor,
                  borderRadius: const BorderRadius.only(
                    topLeft: Radius.circular(25),
                    topRight: Radius.circular(25),
                  ),
                  boxShadow: [
                    BoxShadow(
                      offset: const Offset(0, 4),
                      blurRadius: 40,
                      color: MyColors.shadowColor.withOpacity(0.2),
                    ),
                  ],
                ),
                child: Padding(
                  padding: EdgeInsets.symmetric(
                    horizontal: MediaQuery.of(context).size.width * 0.05,
                    vertical: MediaQuery.of(context).size.height * 0.05,
                  ),
                  child: Form(
                    key: formKey,
                    child: Column(
                      children: [
                        buildPasswordField(
                          controller: oldPasswordController,
                          hintText: 'Eski Şifre',
                          focusNode: oldPasswordFocusNode,
                          isObsecure: isObsecureOld,
                          onTap: () {
                            setState(() {
                              isObsecureOld = !isObsecureOld;
                            });
                          },
                        ),
                        buildPasswordField(
                          controller: newPasswordController,
                          hintText: 'Yeni Şifre',
                          focusNode: newPasswordFocusNode,
                          isObsecure: isObsecureNew,
                          onTap: () {
                            setState(() {
                              isObsecureNew = !isObsecureNew;
                            });
                          },
                        ),
                        buildPasswordField(
                          controller: againPasswordController,
                          hintText: 'Yeni Şifre Tekrar',
                          focusNode: againPasswordFocusNode,
                          isObsecure: isObsecureAgain,
                          onTap: () {
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
                            textColor: MyColors.primaryColor,
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
                                context,
                                isForce: true,
                              );
                            },
                            bgcolor: Colors.white,
                            text: 'Şifreyi Güncelle',
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget buildPasswordField({
    required TextEditingController controller,
    required String hintText,
    required FocusNode focusNode,
    required bool isObsecure,
    required VoidCallback onTap,
  }) {
    return Container(
      margin: const EdgeInsets.symmetric(vertical: 10),
      padding: const EdgeInsets.all(10),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(40),
        boxShadow: [
          BoxShadow(
            offset: const Offset(0, 12),
            blurRadius: 60,
            spreadRadius: 0,
            color: const Color(0xFF43474D).withOpacity(0.06),
          )
        ],
      ),
      child: Row(
        children: [
          SvgPicture.asset(
            AssetsConstant.lockIcon,
            height: MediaQuery.of(context).size.height * 0.045,
          ),
          SizedBox(width: MediaQuery.of(context).size.width * 0.04),
          Flexible(
            child: TextFormField(
              controller: controller,
              focusNode: focusNode,
              keyboardType: TextInputType.text,
              decoration: InputDecoration(
                border: InputBorder.none,
                hintText: hintText,
                hintStyle: Theme.of(context)
                    .textTheme
                    .bodyLarge!
                    .copyWith(color: MyColors.hintColor),
              ),
              obscureText: isObsecure,
              obscuringCharacter: '●',
              textInputAction: TextInputAction.done,
              onFieldSubmitted: (_) {
                FocusScope.of(context).requestFocus(focusNode);
              },
            ),
          ),
          GestureDetector(
            onTap: onTap,
            child: Padding(
              padding: const EdgeInsets.symmetric(horizontal: 10),
              child: SvgPicture.asset(AssetsConstant.eyeOn),
            ),
          ),
        ],
      ),
    );
  }
}
