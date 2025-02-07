import 'package:flutter/material.dart';
import 'package:get/get_state_manager/src/simple/get_state.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/core/extensions/size_extension.dart';

import 'splash_controller.dart';

class SplashPage extends StatelessWidget {
  const SplashPage({super.key});

  @override
  Widget build(BuildContext context) {
    return GetBuilder<SplashController>(
      init: SplashController(),
      initState: (_) {},
      builder: (_) {
        return Container(
          color: Colors.white,
          child: Container(
            margin: EdgeInsets.all(context.dynamicHeight * 0.05),
            child: Image.asset(AssetsConstant.logo),
          ),
        );
      },
    );
  }
}
