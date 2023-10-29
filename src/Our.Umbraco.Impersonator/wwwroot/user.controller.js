(function () {
  "use strict";

  function ImpersonatorUserController(
    $scope,
    $http,
    authResource,
    usersResource,
    notificationsService,
    localizationService
  ) {
    var vm = this;

    vm.isImpersonating = false;
    vm.users = [];
    vm.loadingUsers = true;
    vm.impersonateUserButtonState = "init";
    vm.canImpersonateUsers = false;

    authResource.getCurrentUser().then(function (user) {
      if (user.allowedSections.indexOf("users") > -1) {
        vm.canImpersonateUsers = true;

        // Get users
        usersResource
          .getPagedResults({
            pageSize: 2147483647,
            userStates: ["Active"],
          })
          .then(
            function (data) {
              vm.users = data.items.filter(function (u) { return u.id != user.id }); // remove current user from impersonation list
              vm.loadingUsers = false;
            },
            function (error) {
              vm.loadingUsers = false;
            }
          );
      }
      else {
        // No users to load, we're done
        vm.loadingUsers = false;
      }
    });

    vm.impersonateUser = function () {
      vm.impersonateUserButtonState = "busy";

      $http
        .post(
          "/umbraco/backoffice/Impersonator/User/Impersonate/" +
            vm.userToImpersonate.id
        )
        .then(function success(response) {
          if (response.data == "success") {
            window.location.reload();
          } else {
            vm.impersonateButtonState = "error";
            localizationService
              .localize("impersonator_" + response.data)
              .then(function (value) {
                notificationsService.error(value);
              });
          }
        });
    };

    $http
      .get("/umbraco/backoffice/Impersonator/User/GetImpersonatingUserHash")
      .then(function success(response) {
        if (response.data) {
          vm.isImpersonating = true;
        }
      });

    vm.endImpersonation = function () {
      $http
        .post("/umbraco/backoffice/Impersonator/User/EndImpersonation/")
        .then(function success(response) {
          vm.endImpersonatingButtonState = "busy";
          window.location.reload();
        });
    };
  }

  angular
    .module("umbraco")
    .controller("Impersonator.User.Controller", ImpersonatorUserController);
})();
