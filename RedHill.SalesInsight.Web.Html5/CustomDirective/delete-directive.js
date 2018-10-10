angular
	.module('SIWeb')
        .directive('deleteButton', deleteButton);


	function deleteButton($window) {
		var directive = {
			link: link,
			templateUrl: "/CustomDirective/deleteButton.html",
			restrict: "EA",
			scope: {
				differentEvent: '&',
				index: '@'
			},
			controller: ['$scope', controller]
		};
		return directive;

		function link(scope, element, attrs) {
			var clickedOutsite = false;
			var clickedElement = false;

			$(document).mouseup(function (e) {
				clickedElement = false;
				clickedOutsite = false;
			});

			element.on("mousedown", function (e) {
				clickedElement = true;
				if (!clickedOutsite && clickedElement) {
					scope.$apply(function () {
						//user clicked the element
						
					});
				}

			});

			$(document).mousedown(function (e) {
				clickedOutsite = true;
				if (clickedOutsite && !clickedElement) {
					scope.$apply(function () {
						//user clicked outsite the element 
						scope.state = 0;
					});
				}
			});
		}

		function controller($scope) {
			$scope.states = ["", "Confirm?"];
			$scope.state = 0;
			
			//$scope.aa = $scope.differentevent;
			//console.log($scope.index);
			$scope.deleteAction = function () {
				if ($scope.state === 0) {
					$scope.state = 1;
				} else {
					//Fire the event
					if ($scope.differentEvent && typeof $scope.differentEvent == 'function') {
						$scope.differentEvent();
					}
				}
				//if ($(e.target).attr("data-delete")) {
				//	$scope.quoteAggregateDetails.splice(index, 1);
				//}
			}
		}
    }
