import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:get/get.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/pages/auth/controller/login_controller.dart';
import 'package:mobile/pages/common/common_button.dart';
import 'package:mobile/styles/colors.dart';

class CommonBottomSheet extends StatefulWidget {
  final void Function()? approveOnTap;
  final void Function()? cancelOnTap;
  final String headerText;
  final String smallText;
  final String cancelText;
  final String approveText;
  final Color? approveButtonColor;
  final Color? cancelButtonColor;
  final Color? cancelTextColor;
  final bool isScan;
  const CommonBottomSheet({
    super.key,
    this.approveOnTap,
    this.cancelOnTap,
    this.headerText = 'Emin misiniz?',
    this.smallText = 'Kullanıcı çıkışı yapmaktasınız.',
    this.cancelText = 'İptal',
    this.approveText = 'Çıkış Yap',
    this.cancelButtonColor,
    this.cancelTextColor,
    this.approveButtonColor,
    this.isScan = false,
  });

  @override
  State<CommonBottomSheet> createState() => _CommonBottomSheetState();
}

class _CommonBottomSheetState extends State<CommonBottomSheet> {
  final controller = Get.put(LoginController());
  @override
  Widget build(BuildContext context) {
    return FractionallySizedBox(
      heightFactor: 0.50,
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 24.0, vertical: 10.0),
        child: SingleChildScrollView(
          child: Column(
            children: [
              SvgPicture.asset(AssetsConstant.areusure),
              Text(
                widget.headerText,
                style: Theme.of(context).textTheme.headlineSmall!.copyWith(
                      color: MyColors.darkBlue,
                      fontWeight: FontWeight.w600,
                    ),
              ),
              Text(
                widget.smallText,
                textAlign: TextAlign.center,
                style: Theme.of(context).textTheme.bodyMedium!.copyWith(
                      color: MyColors.darkBlue,
                      fontWeight: FontWeight.w400,
                    ),
              ),
              CommonButton(
                text: widget.approveText,
                textColor: Colors.white,
                bgcolor: widget.approveButtonColor ?? MyColors.primaryColor,
                onTap: widget.approveOnTap ??
                    () {
                      HapticFeedback.mediumImpact();
                      controller.logout(context);
                    },
              ),
              CommonButton(
                text: widget.cancelText,
                textColor: widget.cancelTextColor ?? Colors.white,
                bgcolor: widget.cancelButtonColor ?? const Color(0xFFDD5050),
                onTap: widget.cancelOnTap ??
                    () {
                      HapticFeedback.mediumImpact();
                      Get.back();
                    },
              ),
            ],
          ),
        ),
      ),
    );
  }
}
