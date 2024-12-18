import 'package:email_validator/email_validator.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:get/get.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/pages/auth/controller/login_controller.dart';
import 'package:mobile/pages/common/common_button.dart';
import 'package:mobile/pages/common/common_text_field.dart';
import 'package:mobile/styles/colors.dart';

class ForgotPasswordView extends StatefulWidget {
  const ForgotPasswordView({super.key});

  @override
  State<ForgotPasswordView> createState() => _ForgotPasswordViewState();
}

class _ForgotPasswordViewState extends State<ForgotPasswordView> {
  final emailController = TextEditingController();
  final loginController = Get.put(LoginController());

  @override
  void initState() {
    emailController.addListener(_updateEmailIconColor);
    super.initState();
  }

  @override
  void dispose() {
    super.dispose();
    emailController.removeListener(_updateEmailIconColor);
    emailController.dispose();
    loginController.emailIconColorForgot = const Color(0xFFADB4C0);
    loginController.buttonColorForgot = Colors.white;
    loginController.textColorForgot = const Color(0xFF838FA0);
    loginController.labelTextColorForgot = const Color(0xFF000000);
  }

  void _updateEmailIconColor() {
    final email = emailController.text;
    if (EmailValidator.validate(email)) {
      setState(() {
        loginController.emailIconColorForgot = Colors.green;
        loginController.buttonColorForgot = MyColors.greenColor2;
        loginController.textColorForgot = Colors.white;
        loginController.labelTextColorForgot = Colors.black;
      });
    } else {
      setState(() {
        loginController.emailIconColorForgot = const Color(0xFFADB4C0);
        loginController.buttonColorForgot = Colors.white;
        loginController.textColorForgot = const Color(0xFF838FA0);
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      resizeToAvoidBottomInset: true,
      body: Container(
        color: MyColors.primaryColor,
        child: SingleChildScrollView(
          child: Column(
            children: [
              SizedBox(
                height: context.dynamicHeight * 0.03,
              ),
              Align(
                alignment: Alignment.centerLeft,
                child: IconButton(
                  onPressed: () {
                    Get.back();
                  },
                  icon: const Icon(
                    Icons.arrow_back,
                    color: Colors.white,
                    size: 40,
                  ),
                ),
              ),
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
                height: context.dynamicHeight,
                decoration: BoxDecoration(
                  color: MyColors.primaryColor,
                  borderRadius: const BorderRadius.only(
                      topLeft: Radius.circular(25),
                      topRight: Radius.circular(25)),
                  boxShadow: [
                    BoxShadow(
                      offset: const Offset(0, 4),
                      blurRadius: 40,
                      color: MyColors.shadowColor.withOpacity(0.2),
                    ),
                  ],
                ),
                child: SingleChildScrollView(
                  child: Padding(
                    padding: EdgeInsets.symmetric(
                        horizontal: context.dynamicWidth * 0.05,
                        vertical: context.dynamicHeight * 0.05),
                    child: Column(
                      children: [
                        CommonTextField(
                          controller: emailController,
                          hintText: 'E-mail',
                          iconPath: AssetsConstant.emailIcon,
                          svgColor: loginController.emailIconColorForgot,
                          keyboardType: TextInputType.emailAddress,
                          labelTextColor: loginController.labelTextColorForgot,
                        ),
                        SizedBox(
                          height: context.dynamicHeight * 0.01,
                        ),
                        CommonButton(
                          textColor: loginController.textColorForgot,
                          bgcolor: loginController.buttonColorForgot,
                          text: 'Şifremi Güncelle',
                          onTap: () {
                            HapticFeedback.mediumImpact();
                            loginController
                                .forgotPassword(context, emailController.text)
                                .then((value) {
                              if (value == false) {
                                setState(() {
                                  loginController.buttonColorForgot =
                                      MyColors.redColor2;
                                  loginController.emailIconColorForgot =
                                      MyColors.redColor2;
                                  loginController.labelTextColorForgot =
                                      MyColors.redColor;
                                });
                              }
                            });
                          },
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
}
