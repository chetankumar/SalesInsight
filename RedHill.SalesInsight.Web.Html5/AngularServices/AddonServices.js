(function () {
    'use strict';

    angular
		.module('SIWeb')
		.factory('addonServices', addonServices);

	addonServices.$inject = ['$http'];

	function addonServices($http) {
		var service = {
			concreteAddonList: concreteAddonList
			//  sendData : sendData
		}
		return service;


		function concreteAddonList() {
			return $http.post('/json/GetAllConcreteAddons?quoteId=17').then(function (response) {
				return response;
			}, function () {

			});
		}
	}
})();