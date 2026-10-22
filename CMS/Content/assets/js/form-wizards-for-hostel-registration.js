
var handleBootstrapWizardsValidation = function () {
    "use strict";
    $("#wizard").bwizard({
        validating: function (e, t) {
            if (t.index == 0) {
                if (false === $('#devForm').parsley().validate("wizard-step-1")) {
                    return false
                } else {
                    SaveOrUpdateHostelPersonal();
                }
            } else if (t.index == 1) {
                if (false === $('#devForm').parsley().validate("wizard-step-2")) {
                    return false
                } else {
                    SaveOrUpdateHostelPersonal();
                }
            } else if (t.index == 2) {
                if (false === $('#devForm').parsley().validate("wizard-step-3")) {
                    return false
                } else {
                    SaveOrUpdateFamilyDetails();
                }
            }
            //else if (t.index == 3) {
            //    if (false === $('#devForm').parsley().validate("wizard-step-4")) {
            //        return false
            //    } else {
            //        SaveOrUpdatedPhotoUploads();
            //    }
            //}
        }
    })
};
var FormWizardValidation = function () {
    "use strict";
    return {
        init: function () {
            handleBootstrapWizardsValidation()
        }
    }
}()