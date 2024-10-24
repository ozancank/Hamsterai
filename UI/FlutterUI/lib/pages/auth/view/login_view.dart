import 'package:email_validator/email_validator.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:get/get.dart';
import 'package:mobile/pages/auth/controller/login_controller.dart';
import 'package:mobile/pages/auth/model/login_post_model.dart';
import 'package:mobile/pages/auth/view/forgot_password_view.dart';
import 'package:mobile/pages/common/common_text_field.dart';
import 'package:mobile/pages/common/common_button.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/styles/colors.dart';

class LoginView extends StatefulWidget {
  const LoginView({super.key});

  @override
  State<LoginView> createState() => _LoginViewState();
}

class _LoginViewState extends State<LoginView> {
  final GlobalKey<FormState> formKey = GlobalKey();
  final loginController = Get.put(LoginController());
  final emailController = TextEditingController();
  final passwordController = TextEditingController();

  @override
  void initState() {
    emailController.addListener(_updateIconColors);
    passwordController.addListener(_updateIconColors);
    super.initState();
  }

  @override
  void dispose() {
    emailController.removeListener(_updateIconColors);
    passwordController.removeListener(_updateIconColors);
    emailController.dispose();
    passwordController.dispose();
    loginController.emailIconColor = const Color(0xFFADB4C0);
    loginController.buttonColor = Colors.white;
    loginController.textColor = const Color(0xFF838FA0);
    loginController.labelTextColor = const Color(0xFF000000);
    super.dispose();
  }

  void _updateIconColors() {
    final email = emailController.text;
    final password = passwordController.text;

    final isEmailValid = EmailValidator.validate(email);
    loginController.emailIconColor =
        isEmailValid ? Colors.green : const Color(0xFFADB4C0);

    final isPasswordValid = password.length >= 8;
    loginController.passwordIconColor =
        isPasswordValid ? Colors.green : const Color(0xFFADB4C0);
    loginController.labelTextColor = (isEmailValid && isPasswordValid)
        ? Colors.black
        : const Color(0xFF838FA0);
    if (isEmailValid && isPasswordValid) {
      loginController.buttonColor = MyColors.greenColor;
      loginController.textColor = Colors.white;
    } else {
      loginController.buttonColor = Colors.white;
      loginController.textColor = const Color(0xFF838FA0);
    }
    setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      resizeToAvoidBottomInset: true,
      backgroundColor: MyColors.primaryColor,
      body: Container(
        color: Colors.white,
        child: SingleChildScrollView(
          child: Column(
            children: [
              SizedBox(
                height: context.dynamicHeight * 0.1,
              ),
              Container(
                padding: EdgeInsets.all(context.dynamicHeight * 0.01),
                height: context.dynamicHeight * 0.3,
                width: double.infinity,
                alignment: Alignment.center,
                child: Image.asset(AssetsConstant.logo2),
              ),
              SizedBox(
                height: context.dynamicHeight * 0.03,
              ),
              Container(
                decoration: const BoxDecoration(
                  color: MyColors.primaryColor,
                  borderRadius: BorderRadius.only(
                      topLeft: Radius.circular(25),
                      topRight: Radius.circular(25)),
                ),
                child: SingleChildScrollView(
                  child: Padding(
                    padding: EdgeInsets.symmetric(
                        horizontal: context.dynamicWidth * 0.05,
                        vertical: context.dynamicHeight * 0.04),
                    child: Form(
                      key: formKey,
                      child: Column(
                        children: [
                          CommonTextField(
                            controller: emailController,
                            hintText: 'E-mail',
                            iconPath: AssetsConstant.emailIcon,
                            svgColor: loginController.emailIconColor,
                            keyboardType: TextInputType.emailAddress,
                            labelTextColor: loginController.labelTextColor,
                          ),
                          CommonTextField(
                            controller: passwordController,
                            hintText: 'Şifre',
                            iconPath: AssetsConstant.lockIcon,
                            isObsecure: true,
                            svgColor: loginController.passwordIconColor,
                            isPassword: true,
                            labelTextColor: loginController.labelTextColor,
                          ),
                          SizedBox(
                            height: context.dynamicHeight * 0.01,
                          ),
                          forgotPassword(context),
                          SizedBox(
                            height: context.dynamicHeight * 0.13,
                          ),
                          CommonButton(
                            textColor: loginController.textColor,
                            bgcolor: loginController.buttonColor,
                            text: 'Giriş',
                            onTap: () {
                              HapticFeedback.mediumImpact();
                              if (!formKey.currentState!.validate()) {
                                return;
                              }
                              loginController
                                  .login(
                                LoginPostModel(
                                    userName: emailController.text,
                                    password: passwordController.text,
                                    authenticatorCode: 0),
                                context,
                              )
                                  .then((value) {
                                if (value == false) {
                                  setState(() {
                                    loginController.labelTextColor =
                                        MyColors.redColor;
                                    loginController.emailIconColor =
                                        MyColors.redColor;
                                    loginController.passwordIconColor =
                                        MyColors.redColor;
                                    loginController.buttonColor =
                                        MyColors.redColor;
                                    loginController.textColor = Colors.white;
                                  });
                                }
                              });
                              return;
                            },
                          ),
                        ],
                      ),
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

  GestureDetector iDontHaveAccount(BuildContext context) {
    return GestureDetector(
      onTap: () {
        HapticFeedback.mediumImpact();
      },
      child: Align(
        alignment: Alignment.center,
        child: Text(
          "Hesabın mı yok? Hemen oluştur",
          style: Theme.of(context).textTheme.titleMedium!.copyWith(
                color: Colors.white,
                fontWeight: FontWeight.w600,
              ),
        ),
      ),
    );
  }

  GestureDetector forgotPassword(BuildContext context) {
    return GestureDetector(
      onTap: () {
        HapticFeedback.mediumImpact();
        Get.to(() => const ForgotPasswordView());
      },
      child: Align(
        alignment: Alignment.centerRight,
        child: Text(
          'Şifremi Unuttum',
          style: Theme.of(context).textTheme.titleMedium!.copyWith(
                color: Colors.white,
                fontWeight: FontWeight.w600,
              ),
        ),
      ),
    );
  }
}
