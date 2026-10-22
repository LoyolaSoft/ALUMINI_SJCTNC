using CMS.DAO;

namespace CMS.SQL.Alumnae
{
    public static class AlumnaeSQL
    {
        public static string GetAlumnaeSQL(AlumnaeSQLCommands sEnumCommand)
        {
            string query = "";
            switch (sEnumCommand)
            {
                case AlumnaeSQLCommands.SaveAluRegistrationInfo:
                    {
                        query = @"INSERT 
                                    INTO ALU_REGISTRATION_INFO
                                    (FIRST_NAME, 
                                    LAST_NAME,
                                    BATCH_BEGIN_YEAR,
                                    BATCH_END_YEAR,
                                    PROGRAMME_ID, 
                                    DEPARTMENT,
                                    DATE_OF_BIRTH, 
                                    EMAIL_ID, 
                                    MOBILE_NO, 
                                    ADDRESS,
                                    COUNTRY,
                                    AADHAR_NUMBER,
                                    PAN_NUMBER, 
                                    MARITAL_STATUS,
                                    IS_HIGHER,
                                    IS_RESEARCH,
                                    IS_PLACEMENT,
                                    IS_ENTREPRENEUR,
                                    COMPETITIVE_EXAM,
                                    ANY_OTHER,
                                    ACHIEVEMENT,
                                    VISIT_OUR_CAMPUS,
                                    SUGGESTION,
                                    EXPERIENCE,
                                    CONTRIBUTION_TYPE_ID,
                                    PHOTO,
                                    PNAME_OF_ORGANIZATION,
                                    PCUR_DESIGNATION,
                                    PCONTACT_ADDRESS,
                                    PMOBILE_NO,
                                    PEMAIL_ID,
                                    ACADEMIC_YEAR)
        
                                    VALUES(?FIRST_NAME,
                                    ?LAST_NAME,
                                    ?BATCH_BEGIN_YEAR,
                                    ?BATCH_END_YEAR,
                                    ?PROGRAMME_ID,
                                    ?DEPARTMENT,
                                    ?DATE_OF_BIRTH,
                                    ?EMAIL_ID,
                                    ?MOBILE_NO,
                                    ?ADDRESS,
                                    ?COUNTRY,
                                    ?AADHAR_NUMBER,
                                    ?PAN_NUMBER,
                                    ?MARITAL_STATUS,
                                    ?IS_HIGHER,
                                    ?IS_RESEARCH,
                                    ?IS_PLACEMENT,
                                    ?IS_ENTREPRENEUR,
                                    ?COMPETITIVE_EXAM,
                                    ?ANY_OTHER,
                                    ?ACHIEVEMENT,
                                    ?VISIT_OUR_CAMPUS,
                                    ?SUGGESTION,
                                    ?EXPERIENCE,
                                    ?CONTRIBUTION_TYPE_ID,
                                    ?PHOTO,
                                    ?PNAME_OF_ORGANIZATION,
                                    ?PCUR_DESIGNATION,
                                    ?PCONTACT_ADDRESS,
                                    ?PMOBILE_NO,
                                    ?PEMAIL_ID,
                                    ?AC_YEAR);";
                        break;
                    }
                case AlumnaeSQLCommands.SaveAlumniDataCollectionInfo:
                    {
                        query = @"INSERT INTO ALUMNI_DATA_COLLECTION
                                    (SALUTATION, NAME, ROLL_NO, PROGRAMME, BRANCH_UG, BRANCH_PG, BRANCH_MPHIL, BRANCH_PHD, BATCH, STREAM, PRESENT_STATUS, DESIGNATION, COMPANY, PERMANENT_ADDRESS, COMMUNICATION_ADDRESS, MOBILE_NO, EMAIL_ID, OTHER_DETAILS, DATE_OF_BIRTH, PHOTO, ACADEMIC_YEAR, CREATED_DATE)
                                    VALUES
                                    (?SALUTATION, ?NAME, ?ROLL_NO, ?PROGRAMME, ?BRANCH_UG, ?BRANCH_PG, ?BRANCH_MPHIL, ?BRANCH_PHD, ?BATCH, ?STREAM, ?PRESENT_STATUS, ?DESIGNATION, ?COMPANY, ?PERMANENT_ADDRESS, ?COMMUNICATION_ADDRESS, ?MOBILE_NO, ?EMAIL_ID, ?OTHER_DETAILS, ?DATE_OF_BIRTH, ?PHOTO, ?AC_YEAR, NOW());";
                        break;
                    }
                case AlumnaeSQLCommands.FetchAlumniDataCollectionInfoByRollNo:
                    {
                        query = @"SELECT SALUTATION, NAME, ROLL_NO, PROGRAMME, BRANCH_UG, BRANCH_PG, BRANCH_MPHIL, BRANCH_PHD, BATCH, STREAM, PRESENT_STATUS, DESIGNATION, COMPANY, PERMANENT_ADDRESS, COMMUNICATION_ADDRESS, MOBILE_NO, EMAIL_ID, OTHER_DETAILS, IFNULL(DATE_FORMAT(DATE_OF_BIRTH, '%d-%m-%Y'), DATE_OF_BIRTH) AS DATE_OF_BIRTH, PHOTO
                                  FROM ALUMNI_DATA_COLLECTION 
                                  WHERE ROLL_NO = ?ROLL_NO;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchAlumniDataCollectionInfoByEmail:
                    {
                        query = @"SELECT SALUTATION, NAME, ROLL_NO, PROGRAMME, BRANCH_UG, BRANCH_PG, BRANCH_MPHIL, BRANCH_PHD, BATCH, STREAM, PRESENT_STATUS, DESIGNATION, COMPANY, PERMANENT_ADDRESS, COMMUNICATION_ADDRESS, MOBILE_NO, EMAIL_ID, OTHER_DETAILS, IFNULL(DATE_FORMAT(DATE_OF_BIRTH, '%d-%m-%Y'), DATE_OF_BIRTH) AS DATE_OF_BIRTH, PHOTO
                                  FROM ALUMNI_DATA_COLLECTION 
                                  WHERE EMAIL_ID = ?EMAIL_ID;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchAllAlumniDataCollection:
                    {
                        query = @"SELECT 
                                    A.SALUTATION, 
                                    A.NAME, 
                                    A.ROLL_NO, 
                                    IFNULL(CP.PROGRAMME_NAME, A.PROGRAMME) AS PROGRAMME, 
                                    A.BRANCH_UG, 
                                    A.BRANCH_PG, 
                                    A.BRANCH_MPHIL, 
                                    A.BRANCH_PHD, 
                                    A.BATCH, 
                                    IFNULL(SPS.SHIFT_NAME, A.STREAM) AS STREAM, 
                                    A.PRESENT_STATUS, 
                                    A.DESIGNATION, 
                                    A.COMPANY, 
                                    A.PERMANENT_ADDRESS, 
                                    A.COMMUNICATION_ADDRESS, 
                                    A.MOBILE_NO, 
                                    A.EMAIL_ID, 
                                    A.OTHER_DETAILS, 
                                    IFNULL(DATE_FORMAT(A.DATE_OF_BIRTH, '%d-%m-%Y'), A.DATE_OF_BIRTH) AS DATE_OF_BIRTH, 
                                    A.PHOTO 
                                  FROM ALUMNI_DATA_COLLECTION A
                                  LEFT JOIN CP_PROGRAMME_GROUP CPG ON CPG.PROGRAMME_GROUP_ID = A.PROGRAMME
                                  LEFT JOIN CP_PROGRAMME CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                  LEFT JOIN SUP_SHIFT SPS ON SPS.SHIFT_ID = A.STREAM
                                  ORDER BY A.CREATED_DATE DESC;";
                        break;
                    }
                case AlumnaeSQLCommands.SaveAluHigheDetails:
                    {
                        query = @"INSERT 
                                    INTO
                                    ALU_HIGHER_DETAILS
                                    (ALU_REGISTER_ID,
                                    COURSE,
                                    INSTITUTION,
                                    COUNTRY)

                                    VALUES(?ALU_REGISTER_ID,
                                    ?COURSE,
                                    ?INSTITUTION,
                                    ?COUNTRY);";
                        break;
                    }
                case AlumnaeSQLCommands.SaveAluPlacementDetails:
                    {
                        query = @"INSERT
                                    INTO 
                                    ALU_PLACEMENT_DETAILS
                                    (ALU_REGISTER_ID, 
                                    PRESENT_ORGANIZATION,
                                    POSITION, 
                                    EMP_CONTACTNO,
                                    EMP_MAIL_ID,
                                    EXPERIENCE_IN_PREORGANIZATION)

                                    VALUES(?ALU_REGISTER_ID,
                                    ?PRESENT_ORGANIZATION,
                                    ?POSITION,
                                    ?EMP_CONTACTNO,
                                    ?EMP_MAIL_ID,
                                    ?EXPERIENCE_IN_PREORGANIZATION);";
                        break;
                    }
                case AlumnaeSQLCommands.SaveAluResearchDetails:
                    {
                        query = @"INSERT 
                                INTO 
                                ALU_RESEARCH_DETAILS
                                (ALU_REGISTER_ID,
                                SPECIALIZATION,
                                AREA_OF_RESEARCH,
                                COUNTRY, 
                                INSTITUTION)

                                VALUES(?ALU_REGISTER_ID,
                                ?SPECIALIZATION,
                                ?AREA_OF_RESEARCH,
                                ?COUNTRY,?INSTITUTION);";
                        break;
                    }

                case AlumnaeSQLCommands.SaveAluEntreprenuerDetails:
                    {
                        query = @"INSERT 
                                    INTO
                                    ALU_ENTREPRENEUR_DETAILS
                                    (ALU_REGISTER_ID,
                                    ENT_NAME,
                                    ENT_CONTACT_DETAILS, 
                                    NATURE_OF_ORGANIZATION)

                                    VALUES(?ALU_REGISTER_ID,
                                    ?ENT_NAME,
                                    ?ENT_CONTACT_DETAILS,
                                    ?NATURE_OF_ORGANIZATION);";
                        break;
                    }
                case AlumnaeSQLCommands.SaveAluContributionDetails:
                    {
                        query = @"INSERT 
                                    INTO 
                                    ALU_CONTRIBUTION_DETAILS
                                    (ALU_REGISTER_ID, 
                                    VISIT_OUR_CAMPUS,
                                    SUGGESTION,
                                    EXPERIENCE,
                                    CONTRIBUTION_TYPE_ID)

                                    VALUES(?ALU_REGISTER_ID,
                                    ?VISIT_OUR_CAMPUS,
                                    ?SUGGESTION,
                                    ?EXPERIENCE,
                                    ?CONTRIBUTION_TYPE_ID);";
                        break;
                    }
                case AlumnaeSQLCommands.FetchLastIndexAluRegistrationInfo:
                    {
                        query = @"SELECT 
                                        ALU_REGISTER_ID 
                                        FROM
                                        ALU_REGISTRATION_INFO
                                    WHERE
                                        ALU_REGISTER_ID > 0
                                        AND IS_DELETED!=1
                                    ORDER BY ALU_REGISTER_ID DESC
                                    LIMIT 1;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchAluHigherDetailsByAluRegisterId:
                    {
                        query = @"SELECT 
                                ALU_HIGHER_ID,
                                ALU_REGISTER_ID,
                                COURSE,
                                SPECIALIZATION,
                                INSTITUTION,
                                COUNTRY,
                                IS_DELETED,
                                IS_ACTIVE
                            FROM
                                ALU_HIGHER_DETAILS
                            WHERE
                                ALU_REGISTER_ID = ?ALU_REGISTER_ID
                                AND IS_DELETED!=1;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchAluResearchDetailsByAluRegisterId:
                    {
                        query = @"SELECT 
                                ALU_RESEARCH_ID,
                                ALU_REGISTER_ID,
                                SPECIALIZATION,
                                AREA_OF_RESEARCH,
                                COUNTRY,
                                INSTITUTION,
                                IS_DELETED,
                                IS_ACTIVE
                            FROM
                                ALU_RESEARCH_DETAILS
                            WHERE
                                ALU_REGISTER_ID = ?ALU_REGISTER_ID
                                    AND IS_DELETED != 1;";
                        break;
                    }
                case AlumnaeSQLCommands.Getisalreadyaccountcreated:
                    {
                        query = @"
                                SELECT 
                                STUDENT_ID,HEAD                    
                                    
                                FROM
                                    FEE_STUDENT_ACCOUNT
                                WHERE
                                    DEBIT IS NULL
                                    AND FREQUENCY_ID = ?FREQUENCY_ID
                                    AND STUDENT_ID = ?STUDENT_ID AND IS_DELETED != 1;
                                          ";
                        break;
                    }
                case AlumnaeSQLCommands.GetisAlreadyAccountCreatedbyFeeMode:
                    {
                        query = @"SELECT 
                                        STUDENT_ID, HEAD
                                    FROM
                                        FEE_STUDENT_ACCOUNT AS FSA
                                            INNER JOIN
                                        FEE_FREQUENCY_FEE_MODE AS FFFM ON FFFM.FEE_FREQUENCY_FEE_MODE_ID = FSA.FREQUENCY_ID
                                    WHERE
                                        DEBIT IS NULL AND FFFM.FEE_MODE = ?FEE_MODE
                                            AND STUDENT_ID = ?STUDENT_ID
                                            AND fsa.IS_DELETED != 1;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchAluPlacementDetailsByAluRegisterId:
                    {
                        query = @"SELECT 
                                ALU_PLACEMENT_ID,
                                ALU_REGISTER_ID,
                                ORG_NAME,
                                ORG_CONTACT_DETAILS,
                                PRESENT_ORGANIZATION,
                                POSITION,
                                EMP_CONTACTNO,
                                EMP_MAIL_ID,
                                EXPERIENCE_IN_PREORGANIZATION,
                                IS_DELETED,
                                IS_ACTIVE
                            FROM
                                ALU_PLACEMENT_DETAILS
                            WHERE
                                ALU_REGISTER_ID = ?ALU_REGISTER_ID
                                    AND IS_DELETED != 1;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchAluEntreprenuerDetailsByAluRegisterId:
                    {
                        query = @"SELECT 
                                    ALU_ENTREPRENEUR_ID,
                                    ALU_REGISTER_ID,
                                    ENT_NAME,
                                    ENT_CONTACT_DETAILS,
                                    NATURE_OF_ORGANIZATION,
                                    EXPERTISE,
                                    IS_DELETED,
                                    IS_ACTIVE
                                FROM
                                    ALU_ENTREPRENUER_DETAILS
                                WHERE
                                    ALU_REGISTER_ID = ?ALU_REGISTER_ID
                                        AND IS_DELETED != 1;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchAluRegistrationInfoByBatchandProgrammeandDepartment:
                    {
                        query = @"
                                     SELECT 
                                    ARI.ALU_REGISTER_ID,
                                    CONCAT(IFNULL(FIRST_NAME,''),'',IFNULL(LAST_NAME,'')) AS FIRST_NAME,
                                    SB.BATCH AS BATCH,
                                      CONCAT(IFNULL(P.PROGRAMME_NAME, ''),
                                                ' (SHIFT - ',
                                                IFNULL(SH.SHIFT_NAME, ''),
                                                ') ',
                                                IFNULL(SPM.PROGRAMME_MODE, '')) AS PROGRAMME_NAME,
                                    CD.DEPARTMENT,
                                    ARI.DATE_OF_BIRTH,
                                    ARI.EMAIL_ID,
                                    ARI.MOBILE_NO,
                                    ARI.ADDRESS,
                                    ARI.COUNTRY,
                                    ARI.AADHAR_NUMBER,
                                    ARI.PAN_NUMBER,
                                    ARI.IS_HIGHER,
                                    ARI.IS_RESEARCH,
                                    ARI.IS_PLACEMENT,
                                    ARI.IS_ENTREPRENEUR,
                                    ARI.COMPETITIVE_EXAM,
                                    ARI.ANY_OTHER,
                                    ARI.VISIT_OUR_CAMPUS,
                                    ARI.SUGGESTION,
                                    ARI.EXPERIENCE,
                                    group_concat(IFNULL(ST.CONTRIBUTION_TYPE_NAME,'')) AS CONTRIBUTION_TYPE_NAME,
                                    ARI.ACADEMIC_YEAR
                                FROM
                                    ALU_REGISTRATION_INFO AS ARI
                                    INNER JOIN
                                    CP_PROGRAMME_GROUP AS CP ON CP.PROGRAMME_GROUP_ID=ARI.PROGRAMME_ID
                                    AND CP.IS_DELETED!=1
                                    INNER JOIN
                                    CP_PROGRAMME AS P ON P.PROGRAMME_ID = CP.PROGRAMME_ID
                                            AND P.IS_DELETED != 1
									INNER JOIN
                                        SUP_SHIFT SH ON SH.SHIFT_ID = CP.SHIFT
                                            INNER JOIN
                                        SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID = CP.PROGRAMME_MODE
								    INNER JOIN
                                    CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID=ARI.DEPARTMENT
                                    AND CD.IS_DELETED!=1
                                    INNER JOIN
                                    SUP_BATCHES AS SB ON SB.BATCH_ID=ARI.BATCH
                                    AND SB.IS_DELETED!=1
                                    INNER JOIN
                                     sup_alu_contribution_type AS ST ON find_in_set(ST.CONTRIBUTION_TYPE_ID,ARI.CONTRIBUTION_TYPE_ID)
                                        AND ST.IS_DELETED != 1
                                WHERE
                                    ARI.BATCH =?BATCH AND ARI.PROGRAMME_ID=?PROGRAMME_ID
                                        AND ARI.DEPARTMENT=?DEPARTMENT
                                        AND ARI.IS_DELETED != 1
                                        AND ARI.ACADEMIC_YEAR=?AC_YEAR
                                        GROUP BY ARI.ALU_REGISTER_ID;";
                          break;
                    }
                case AlumnaeSQLCommands.FetchAluHigherDetailsByBulkAluRegistraionId:
                    {
                        query = @"SELECT 
                                    ALU_HIGHER_ID,
                                    ALU_REGISTER_ID,
                                    COURSE,
                                    INSTITUTION,
                                    C.COUNTRY_NAME AS COUNTRY
                                FROM
                                    ALU_HIGHER_DETAILS AS AHD
    
                                    INNER JOIN
                                    SUP_COUNTRY AS C ON C.COUNTRY_ID=AHD.COUNTRY
                                    AND C.IS_DELETED!=1
    
                                WHERE
                                    ALU_REGISTER_ID IN (?ALU_REGISTER_ID) AND AHD.IS_DELETED != 1;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchAluResearchDetailsByBulkAluRegistraionId:
                    {
                        query = @"SELECT 
                                    ALU_RESEARCH_ID,
                                    ALU_REGISTER_ID,
                                    SPECIALIZATION,
                                    AREA_OF_RESEARCH,
                                    C.COUNTRY_NAME AS COUNTRY,
                                    INSTITUTION
                                FROM
                                    ALU_RESEARCH_DETAILS AS ARD
                                        INNER JOIN
                                    SUP_COUNTRY AS C ON C.COUNTRY_ID = ARD.COUNTRY
                                        AND C.IS_DELETED != 1
                                WHERE
                                    ALU_REGISTER_ID IN (?ALU_REGISTER_ID)
                                        AND ARD.IS_DELETED != 1;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchAluPlacementDetailsByBulkAluRegistraionId:
                    {
                        query = @"SELECT 
                                    ALU_PLACEMENT_ID,
                                    ALU_REGISTER_ID,
                                    PRESENT_ORGANIZATION,
                                    POSITION,
                                    EMP_CONTACTNO,
                                    EMP_MAIL_ID,
                                    EXPERIENCE_IN_PREORGANIZATION
                                FROM
                                    ALU_PLACEMENT_DETAILS AS APD
    
                                WHERE
                                    ALU_REGISTER_ID IN (?ALU_REGISTER_ID)
                                        AND APD.IS_DELETED != 1;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchAluEntrepreneurDetailsByBulkAluRegistraionId:
                    {
                        query = @"SELECT 
                                    ALU_ENTREPRENEUR_ID,
                                    ALU_REGISTER_ID,
                                    ENT_NAME,
                                    ENT_CONTACT_DETAILS,
                                    NATURE_OF_ORGANIZATION
                                FROM
                                    alu_entrepreneur_details AS AED
                                WHERE
                                    ALU_REGISTER_ID IN (?ALU_REGISTER_ID)
                                        AND AED.IS_DELETED != 1;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchAluContributionDetailsByBulkAluRegisterId:
                    {
                        query = @"
                                SELECT 
                                    ACD.ALU_CONTIBUTION_ID,
                                    ACD.ALU_REGISTER_ID,
                                    ACD.VISIT_OUR_CAMPUS,
                                    ACD.SUGGESTION,
                                    ACD.EXPERIENCE,
                                    group_concat(IFNULL(ST.CONTRIBUTION_TYPE_NAME,'')) AS CONTRIBUTION_TYPE_NAME
                                FROM
                                    alu_contribution_details ACD
                                        INNER JOIN
                                    alu_registration_info AS ARI ON ARI.ALU_REGISTER_ID = ACD.ALU_REGISTER_ID
                                        INNER JOIN
                                    sup_alu_contribution_type AS ST ON ST.CONTRIBUTION_TYPE_ID = ACD.ALU_CONTIBUTION_ID
                                        AND ST.IS_DELETED != 1
                                WHERE
                                    ACD.ALU_REGISTER_ID IN (?ALU_REGISTER_ID)
                                        AND ACD.IS_DELETED != 1
                                        GROUP BY ACD.ALU_REGISTER_ID;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchAluRegistrationInfoByProgrammeandBatchandEmail:
                    {
                        query = @"     
                            SELECT 
                            ALU_REGISTER_ID
                            
                            FROM ALU_REGISTRATION_INFO 
                            WHERE 
                            PROGRAMME_ID=?PROGRAMME_ID
                            AND BATCH_BEGIN_YEAR=?BATCH_BEGIN_YEAR
                            AND EMAIL_ID=?EMAIL_ID
                            AND IS_DELETED!=1;";
                        break;
                    }

                case AlumnaeSQLCommands.FetchProgrammeGroup:
                    {
                        query = @" SELECT 
                                        AP.PROGRAMME_GROUP_ID,
                                        P.PROGRAMME_ID,
                                        CONCAT(IFNULL(P.PROGRAMME_NAME, ''))  AS PROGRAMME_NAME
                                    FROM
                                        CP_PROGRAMME_GROUP AS AP
                                            INNER JOIN
                                        CP_PROGRAMME AS P ON P.PROGRAMME_ID = AP.PROGRAMME_ID
                                            AND P.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_SHIFT SH ON SH.SHIFT_ID = AP.SHIFT
                                            INNER JOIN
                                        SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID = AP.PROGRAMME_MODE
                                    WHERE
                                        AP.IS_DELETED != 1;                  ";
                        break;
                    }
                case AlumnaeSQLCommands.FetchCertificateRequestedByStudentId:
                    {
                        query = @"SELECT 
                                        MSD.CERTIFICATE_DISTRIBUTION_ID,
                                        MSD.STUDENT_ID,
                                        MSD.STATUS,
                                        MSD.REASON,
                                        MSD.AUTHORIZE_REASON,
                                        MONTH_OF_SEMESTER,
                                        YEAR_OF_SEMESTER,
                                        MSD.CONTACT_NO,
                                        MSD.AUTHORIZE_PERSON_NAME,
                                        MSD.AUTHORIZE_PERSON,
                                        IFNULL(S.STATUS, 'Pending') AS STATUS_NAME,
                                        SS.CERTIFICATE_NAME,
                                        DATE_FORMAT(REQUEST_DATE, '%Y/%m/%d') AS REQUEST_DATE,
                                        DATE_FORMAT(APPROVED_DATE, '%Y/%m/%d') AS APPROVED_DATE,
                                        DATE_FORMAT(TO_BE_COLLECTED_DATE, '%Y/%m/%d') AS TO_BE_COLLECTED_DATE,
                                        DATE_FORMAT(ISSUED_DATE, '%Y/%m/%d') AS ISSUED_DATE,
                                        CERTIFICATE_STATUS as CERTIFICATE_STATUS
                                    FROM
                                        COE_CERTIFICATE_DISTRIBUTION AS MSD
                                            INNER JOIN
                                        SUP_CERTIFICATE_LIST AS SS ON SS.CERTIFICATE_ID = MSD.CERTIFICATE_ID
                                            LEFT JOIN
                                        SUP_STATUS AS S ON S.STATUS_ID = MSD.STATUS
                                    WHERE
                                        MSD.IS_DELETED != 1 AND MSD.STUDENT_ID=?STUDENT_ID AND SS.IS_DELETED!=1;";
                        break;
                    }

                #region supcertificate
                case AlumnaeSQLCommands.FetchSupCertificateList:
                    {
                        query = @"SELECT 
                                        CERTIFICATE_ID,
                                        CERTIFICATE_NAME
                                    FROM
                                        SUP_CERTIFICATE_LIST
                                    WHERE
                                        IS_DELETED != 1 AND IS_ACTIVE = 1 ORDER BY ORDER_BY;";
                        break;
                    }
                #endregion
                case AlumnaeSQLCommands.DeleteCertificateRequested:
                    {
                        query = @"UPDATE COE_CERTIFICATE_DISTRIBUTION 
                                    SET 
                                        IS_DELETED = 1
                                    WHERE
                                        CERTIFICATE_DISTRIBUTION_ID = ?CERTIFICATE_DISTRIBUTION_ID;";
                        break;
                    }
                case AlumnaeSQLCommands.UpdateCertificateRequest:
                    {
                        query = @"UPDATE coe_certificate_distribution 
                                SET 
                                    CERTIFICATE_ID = ?CERTIFICATE_ID,
                                    AUTHORIZE_PERSON_NAME = ?AUTHORIZE_PERSON_NAME,
                                    CONTACT_NO = ?CONTACT_NO,
                                    YEAR_OF_SEMESTER = ?YEAR_OF_SEMESTER,
                                    EXTRA_INFORMATION = ?EXTRA_INFORMATION,
                                    REGISTER_NO = ?REGISTER_NO,
                                    AUTHORIZE_REASON = ?AUTHORIZE_REASON,
                                    MONTH_OF_SEMESTER = ?MONTH_OF_SEMESTER,
                                    CERTIFICATE_TYPE = ?CERTIFICATE_TYPE,
                                    AUTHORIZE_PERSON = ?AUTHORIZE_PERSON,
                                    MODIFIED_BY = ?MODIFIED_BY,
                                    IS_CLEARED_ALL_SUBJECT = ?IS_CLEARED_ALL_SUBJECT
                                WHERE
                                    CERTIFICATE_DISTRIBUTION_ID = ?CERTIFICATE_DISTRIBUTION_ID;";
                        break;
                    }
                case AlumnaeSQLCommands.SaveCertificateRequest:
                    {
                        query = @"INSERT INTO COE_CERTIFICATE_DISTRIBUTION(
                                                                    STUDENT_ID,
                                                                    CERTIFICATE_ID,
                                                                    REGISTER_NO,
                                                                    CONTACT_NO,
                                                                    MONTH_OF_SEMESTER,
                                                                    YEAR_OF_SEMESTER,
                                                                    IS_CLEARED_ALL_SUBJECT,
                                                                    AUTHORIZE_PERSON_NAME,
                                                                    AUTHORIZE_REASON,
                                                                    AUTHORIZE_PERSON,
                                                                    CERTIFICATE_TYPE,
                                                                    CREATED_BY,
                                                                    EXTRA_INFORMATION,
                                                                    ACADEMIC_YEAR) 
                                                                VALUES(
                                                                    ?STUDENT_ID,
                                                                    ?CERTIFICATE_ID,
                                                                    ?REGISTER_NO,
                                                                    ?CONTACT_NO,
                                                                    ?MONTH_OF_SEMESTER,
                                                                    ?YEAR_OF_SEMESTER,
                                                                    ?IS_CLEARED_ALL_SUBJECT,
                                                                    ?AUTHORIZE_PERSON_NAME,
                                                                    ?AUTHORIZE_REASON,
                                                                    ?AUTHORIZE_PERSON,
                                                                    ?CERTIFICATE_TYPE,
                                                                    ?CREATED_BY,
                                                                    ?EXTRA_INFORMATION,
                                                                    ?ACADEMIC_YEAR);";
                        break;
                    }
                #region alumnae passedout student registration
                case AlumnaeSQLCommands.SaveAluPassedoutRegistrationInfo:
                    {
                        query= @"INSERT INTO alu_passedout_register_info
                                (FIRST_NAME,
                                LAST_NAME,
                                PROGRAMME_ID,
                                BATCH_BEGIN_YEAR,
                                BATCH_END_YEAR,
                                DEPARTMENT,
                                DATE_OF_BIRTH,
                                DOB_PROOF,
                                EMAIL_ID,
                                MOBILE_NO,
                                PERMANENT_ADDRESS,
                                SCHOLARSHIP,
                                SCHEME,
                                AMOUNT_RECEIVED,
                                SCHOLOR_BANK_PROOF,
                                AADHAR_NUMBER,
                                PAN_NUMBER,
                                IS_HIGHER,
                                IS_PLACEMENT,
                                NAME_OF_COMPETITIVE_EXAM,
                                EXAM_REG_NO,
                                COMPETITIVE_EXAM_PROOF,
                                IS_CULTURAL,
                                IS_SPORTS,
                                CC_YEAR,
                                CC_NAME,
                                CC_PROOF,
                                SUGGESTION,
                                EXPERIENCE,
                                ACADEMIC_YEAR,
                                REG_DATE)

                                VALUES

                                (?FIRST_NAME,
                                ?LAST_NAME,
                                ?PROGRAMME_ID,
                                ?BATCH_BEGIN_YEAR,
                                ?BATCH_END_YEAR,
                                ?DEPARTMENT,
                                ?DATE_OF_BIRTH,
                                ?DOB_PROOF,
                                ?EMAIL_ID,
                                ?MOBILE_NO,
                                ?PERMANENT_ADDRESS,
                                ?SCHOLARSHIP,
                                ?SCHEME,
                                ?AMOUNT_RECEIVED,
                                ?SCHOLOR_BANK_PROOF,
                                ?AADHAR_NUMBER,
                                ?PAN_NUMBER,
                                ?IS_HIGHER,
                                ?IS_PLACEMENT,
                                ?NAME_OF_COMPETITIVE_EXAM,
                                ?EXAM_REG_NO,
                                ?COMPETITIVE_EXAM_PROOF,
                                ?IS_CULTURAL,
                                ?IS_SPORTS,
                                ?CC_YEAR,
                                ?CC_NAME,
                                ?CC_PROOF,
                                ?SUGGESTION,
                                ?EXPERIENCE,
                                ?AC_YEAR,
                                NOW());";
                        break;
                    }
                case AlumnaeSQLCommands.SaveAluPassedoutHigherDetails:
                    {
                        query = @"INSERT INTO
                                    alu_passedout_higher_details
                                    (ALU_PASSEDOUT_REGISTER_ID,
                                    COURSE,
                                    ID_PROOF)
                                    VALUES
                                    (?ALU_PASSEDOUT_REGISTER_ID,
                                    ?COURSE,
                                    ?ID_PROOF);";
                        break;
                    }
                case AlumnaeSQLCommands.SaveAluPassedoutPlacementDetails:
                    {
                        query = @"INSERT INTO alu_passedout_placement_details
                                    (ALU_PASSEDOUT_REGISTER_ID,
                                    STATUS)
                                    VALUES
                                    (
                                    ?ALU_PASSEDOUT_REGISTER_ID,
                                    ?STATUS);";
                        break;
                    }
                case AlumnaeSQLCommands.SaveAluPassedoutPlacementOffcampusDetails:
                    {
                        query = @"INSERT INTO alu_passedout_pla_offcampus_details
                                    (
                                    ALU_PASSEDOUT_PLACEMENT_ID,
                                    COMPANY_NAME,
                                    PACKAGE_RECEIVED,
                                    ADDRESS,
                                    PROOF)
                                    VALUES
                                    (?ALU_PASSEDOUT_PLACEMENT_ID,
                                    ?COMPANY_NAME,
                                    ?PACKAGE_RECEIVED,
                                    ?ADDRESS,
                                    ?PROOF);";
                        break;
                    }
                case AlumnaeSQLCommands.SaveAluPassedoutPlacementOncampusDetails:
                    {
                        query = @"INSERT INTO 
                                 alu_passedout_pla_oncampus_details
                                (
                                 ALU_PASSEDOUT_PLACEMENT_ID,
                                COMPANY_NAME,
                                PACKAGE_RECEIVED,
                                ADDRESS,
                                PROOF)
                                VALUES
                                (
                                ?ALU_PASSEDOUT_PLACEMENT_ID,
                                ?COMPANY_NAME,
                                ?PACKAGE_RECEIVED,
                                ?ADDRESS,
                                ?PROOF);";
                        break;
                    }
                case AlumnaeSQLCommands.SaveAluPassedoutCulturalDetails:
                    {
                        query = @"INSERT INTO alu_passedout_cultural_details
                                    (
                                    ALU_PASSEDOUT_REGISTER_ID,
                                    NAME_OF_EVENT,
                                    PLACE,
                                    CLGORINSTITUTION_NAME,
                                    PROOF)
                                    VALUES
                                    (
                                    ?ALU_PASSEDOUT_REGISTER_ID,
                                    ?NAME_OF_EVENT,
                                    ?PLACE,
                                    ?CLGORINSTITUTION_NAME,
                                    ?PROOF);";
                        break;
                    }
                case AlumnaeSQLCommands.SaveAluPassedoutSportsDetails:
                    {
                        query = @"INSERT INTO 
                                    alu_passedout_sports_details
                                    (
                                    ALU_PASSEDOUT_REGISTER_ID,
                                    NAME_OF_EVENT,
                                    PROOF)
                                    VALUES
                                    (
                                    ?ALU_PASSEDOUT_REGISTER_ID,
                                    ?NAME_OF_EVENT,
                                    ?PROOF);";
                        break;
                    }
                case AlumnaeSQLCommands.FetchLastIndexAluPassedoutRegistrationInfo:
                    {
                        query = @"SELECT 
                                        ALU_PASSEDOUT_REGISTER_ID 
                                        FROM
                                        alu_passedout_register_info
                                    WHERE
                                        ALU_PASSEDOUT_REGISTER_ID > 0
                                        AND IS_DELETED!=1
                                    ORDER BY ALU_PASSEDOUT_REGISTER_ID DESC
                                    LIMIT 1;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchAluPassedoutHigherDetailsByAluPassedoutRegisterId:
                    {
                        query = @"SELECT 
                                ALU_PASSEDOUT_HIGHER_ID,
                                ALU_PASSEDOUT_REGISTER_ID,
                                COURSE,
                                ID_PROOF
                            FROM
                                alu_passedout_higher_details
                            WHERE
                                ALU_PASSEDOUT_REGISTER_ID = ?ALU_PASSEDOUT_REGISTER_ID
                                AND IS_DELETED!=1;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchAluPassedoutPlacementDetailsByAluPassedoutRegisterId:
                    {
                        query = @"SELECT 
                                ALU_PASSEDOUT_PLACEMENT_ID,
                                ALU_PASSEDOUT_REGISTER_ID,
                                STATUS
                            FROM
                                alu_passedout_placement_details
                            WHERE
                                ALU_PASSEDOUT_REGISTER_ID = ?ALU_PASSEDOUT_REGISTER_ID
                                    AND IS_DELETED != 1;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchLastIndexAluPassedoutPlacementInfo:
                    {
                        query = @"SELECT 
                                        ALU_PASSEDOUT_PLACEMENT_ID,
                                        STATUS
                                        FROM
                                        alu_passedout_placement_details
                                    WHERE
                                        ALU_PASSEDOUT_PLACEMENT_ID > 0
                                        AND IS_DELETED!=1
                                    ORDER BY ALU_PASSEDOUT_PLACEMENT_ID DESC
                                    LIMIT 1;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchAluPassedoutPlaOncampusDetailsByAluPassedoutPlacementId:
                    {
                        query = @"SELECT 
                                ALU_PASSEDOUT_PLACEMENT_ONCAMPUS_ID,
                                ALU_PASSEDOUT_PLACEMENT_ID,
                                COMPANY_NAME,
                                PACKAGE_RECEIVED,
                                ADDRESS,
                                PROOF
                            FROM
                                alu_passedout_pla_oncampus_details
                            WHERE
                                ALU_PASSEDOUT_PLACEMENT_ID = ?ALU_PASSEDOUT_PLACEMENT_ID
                                    AND IS_DELETED != 1;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchAluPassedoutPlaOffcampusDetailsByAluPassedoutPlacementId:
                    {
                        query = @"SELECT 
                                ALU_PASSEDOUT_PLACEMENT_OFFCAMPUS_ID,
                                ALU_PASSEDOUT_PLACEMENT_ID,
                                COMPANY_NAME,
                                PACKAGE_RECEIVED,
                                ADDRESS,
                                PROOF
                            FROM
                                alu_passedout_pla_offcampus_details
                            WHERE
                                ALU_PASSEDOUT_PLACEMENT_ID = ?ALU_PASSEDOUT_PLACEMENT_ID
                                    AND IS_DELETED != 1;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchAluPassedoutCulturalDetailsByAluPassedoutRegisterId:
                    {
                        query = @"SELECT 
                                ALU_PASSEDOUT_CULTURAL_ID,
                                ALU_PASSEDOUT_REGISTER_ID,
                                NAME_OF_EVENT,
                                PLACE,
                                CLGORINSTITUTION_NAME,
                                PROOF
                            FROM
                                alu_passedout_cultural_details
                            WHERE
                                ALU_PASSEDOUT_REGISTER_ID = ?ALU_PASSEDOUT_REGISTER_ID
                                    AND IS_DELETED != 1;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchAluPassedoutSportsDetailsByAluPassedoutRegisterId:
                    {
                        query = @"SELECT 
                                ALU_PASSEDOUT_SPORTS_ID,
                                ALU_PASSEDOUT_REGISTER_ID,
                                NAME_OF_EVENT,
                                PROOF
                            FROM
                                alu_passedout_sports_details
                            WHERE
                                ALU_PASSEDOUT_REGISTER_ID = ?ALU_PASSEDOUT_REGISTER_ID
                                    AND IS_DELETED != 1;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchAluPassedoutRegistrationInfoByProgrammeandBatchandEmail:
                    {
                        query = @"SELECT 
                            ALU_PASSEDOUT_REGISTER_ID
                            
                            FROM alu_passedout_register_info 
                            WHERE 
                            PROGRAMME_ID=?PROGRAMME_ID
                            AND BATCH_BEGIN_YEAR=?BATCH_BEGIN_YEAR
                            AND EMAIL_ID=?EMAIL_ID
                            AND IS_DELETED!=1;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchCertificateRquestInfoById:
                    {
                        query = @"SELECT 
                                        MSD.CERTIFICATE_DISTRIBUTION_ID,
                                        MSD.STUDENT_ID,
                                        MSD.REGISTER_NO,
                                        MSD.STATUS,
                                        MSD.REASON,
                                        MSD.CERTIFICATE_TYPE,
                                        MSD.CONTACT_NO,
                                        IFNULL(S.STATUS, 'Pending') AS STATUS_NAME,
                                        SS.CERTIFICATE_NAME,
                                        DATE_FORMAT(REQUEST_DATE, '%Y/%m/%d') AS REQUEST_DATE,
                                        DATE_FORMAT(APPROVED_DATE, '%Y/%m/%d') AS APPROVED_DATE,
                                        DATE_FORMAT(ISSUED_DATE, '%Y/%m/%d') AS ISSUED_DATE,
                                        CERTIFICATE_STATUS as CERTIFICATE_STATUS,
                                        MONTH_OF_SEMESTER,
                                        YEAR_OF_SEMESTER,
                                        AUTHORIZE_PERSON,
                                        AUTHORIZE_PERSON_NAME,
                                        AUTHORIZE_REASON,
										MSD.CERTIFICATE_ID
                                    FROM
                                        COE_CERTIFICATE_DISTRIBUTION AS MSD
                                            INNER JOIN
                                        SUP_CERTIFICATE_LIST AS SS ON SS.CERTIFICATE_ID = MSD.CERTIFICATE_ID
                                            LEFT JOIN
                                        SUP_STATUS AS S ON S.STATUS_ID = MSD.STATUS
                                    WHERE
                                        MSD.IS_DELETED != 1 AND MSD.CERTIFICATE_DISTRIBUTION_ID=?CERTIFICATE_DISTRIBUTION_ID;";
                        break;
                    }
                    case AlumnaeSQLCommands.FetchCertificateRequestedList:
                    {
                        query = @"SELECT 
                                        SP.FIRST_NAME,
                                        SP.ROLL_NO,
                                        IFNULL(MSD.REGISTER_NO,SP.REGISTER_NO) AS REGISTER_NO,
                                        CC.CLASS_ID,
                                        CP.PROGRAMME_NAME,
                                        CC.CLASS_NAME,
                                        MSD.CERTIFICATE_DISTRIBUTION_ID,
                                        MSD.STUDENT_ID,
                                        MSD.REASON,
                                        MSD.STATUS,
                                        IFNULL(S.STATUS, 'Pending') AS STATUS_NAME,
                                        SS.CERTIFICATE_NAME,
                                        SCCL.CATEGORY_NAME,
                                        CONTACT_NO,
                                        MONTH_OF_SEMESTER,
                                        YEAR_OF_SEMESTER,
                                        IS_CLEARED_ALL_SUBJECT,
                                        AUTHORIZE_PERSON_NAME,
                                        AUTHORIZE_REASON,
                                        AUTHORIZE_PERSON,
                                        SCAP.AUTHORIZER_NAME,
                                        DATE_FORMAT(MSD.REQUEST_DATE, '%Y/%m/%d') AS REQUEST_DATE,
                                        DATE_FORMAT(MSD.APPROVED_DATE, '%Y/%m/%d') AS APPROVED_DATE
                                    FROM
                                        COE_CERTIFICATE_DISTRIBUTION AS MSD
                                            INNER JOIN
                                        (SELECT 
                                            STUDENT_ID, MAX(ACADEMIC_YEAR) AS MAX_YEAR
                                        FROM
                                            STU_CLASS
                                        WHERE
                                            IS_DELETED != 1
                                        GROUP BY STUDENT_ID) AS SC_MAX ON SC_MAX.STUDENT_ID = MSD.STUDENT_ID
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SC_MAX.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = SC_MAX.MAX_YEAR
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            INNER JOIN
                                        SUP_CERTIFICATE_LIST AS SS ON SS.CERTIFICATE_ID = MSD.CERTIFICATE_ID
                                            LEFT JOIN
                                        SUP_STATUS AS S ON S.STATUS_ID = MSD.STATUS
                                            LEFT JOIN
                                        SUP_CERTIFICATE_AUTHORIZE_PERSON AS SCAP ON SCAP.AUTHORIZER_ID = AUTHORIZE_PERSON
                                            LEFT JOIN
                                        SUP_CERTIFICATE_CATEGORY_LIST AS SCCL ON SCCL.CATEGORY_ID = MSD.CERTIFICATE_TYPE
                                    WHERE
                                        MSD.IS_DELETED != 1
                                            AND S.STATUS_ID IS NULL
                                    ORDER BY CERTIFICATE_DISTRIBUTION_ID;";
                        break;
                    }
                case AlumnaeSQLCommands.UpdateCertificateRequestStatus:
                    {
                        query = @"UPDATE COE_CERTIFICATE_DISTRIBUTION 
                                                             SET  
                                                               STATUS=?STATUS,
                                                               EXTRA_INFORMATION=?EXTRA_INFORMATION,
                                                               APPROVER_ID=?APPROVER_ID,
                                                               APPROVED_DATE=?APPROVED_DATE,
                                                               TO_BE_COLLECTED_DATE=?TO_BE_COLLECTED_DATE,
                                                               REASON=?REASON
                                                           WHERE    
                                                                CERTIFICATE_DISTRIBUTION_ID=?CERTIFICATE_DISTRIBUTION_ID;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchCertificateRequestListForSendEmail:
                    {

                        query = @"SELECT 
                                        ccd.STUDENT_ID,
                                        SCL.CERTIFICATE_NAME,
                                        SPI.FIRST_NAME,
                                        ROLL_NO,
                                        CCD.MONTH_OF_SEMESTER,
                                        CCD.YEAR_OF_SEMESTER,
                                        CCD.CONTACT_NO
                                    FROM
                                        COE_CERTIFICATE_DISTRIBUTION AS CCD
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = CCD.STUDENT_ID
                                            INNER JOIN
                                        SUP_CERTIFICATE_LIST AS SCL ON SCL.CERTIFICATE_ID = CCD.CERTIFICATE_ID
                                    WHERE
                                        CCD.IS_DELETED != 1
                                            AND ccd.STUDENT_ID = ?STUDENT_ID
                                            AND CERTIFICATE_DISTRIBUTION_ID = ?CERTIFICATE_DISTRIBUTION_ID;";
                        break;
                    }
                case AlumnaeSQLCommands.SaveCertificateIssueLeave:
                    {

                        query = @"INSERT INTO COE_CERTIFICATE_ISSUE_LEAVE (
                                                                        ENTRY_ID,
                                                                        FROM_DATE,
                                                                        TO_DATE,
                                                                        ACADEMIC_YEAR,
                                                                        REASON
                                                                    )VALUES(
                                                                        ?ENTRY_ID,
                                                                        ?FROM_DATE,
                                                                        ?TO_DATE,
                                                                        ?ACADEMIC_YEAR,
                                                                        ?REASON);";
                        break;
                    }
                case AlumnaeSQLCommands.UpdateCertificateIssueLeave:
                    {

                        query = @"update COE_CERTIFICATE_ISSUE_LEAVE 
                                                            SET 
                                                                UPDATED_BY=?UPDATED_BY,
                                                                FROM_DATE=?FROM_DATE,
                                                                TO_DATE=?TO_DATE,
                                                                REASON=?REASON,
                                                                UPDATED_DATE=?UPDATED_DATE 
                                                            WHERE 
                                                                LEAVEREQUEST_ID=?LEAVEREQUEST_ID";
                        break;
                    }
                case AlumnaeSQLCommands.FetchCertificateIssueLeaveDate:
                    {

                        query = @"SELECT 
                                    FROM_DATE,
                                    TO_DATE,
                                    REASON
                                FROM
                                    COE_CERTIFICATE_ISSUE_LEAVE
                                WHERE
                                    CURRENT_DATE() BETWEEN FROM_DATE AND TO_DATE and IS_DELETED !=1;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchCertificateRequestedListDateWise:
                    {

                        query = @"SELECT 
                                    SP.FIRST_NAME,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    CC.CLASS_ID,
                                    CP.PROGRAMME_NAME,
                                    CC.CLASS_NAME,
                                    MSD.CERTIFICATE_DISTRIBUTION_ID,
                                    MSD.STUDENT_ID,
                                    MSD.REASON,
                                    MSD.STATUS,
                                    IFNULL(S.STATUS, 'Pending') AS STATUS_NAME,
                                    SS.CERTIFICATE_NAME,
                                    CONTACT_NO,
                                    MONTH_OF_SEMESTER,
                                    YEAR_OF_SEMESTER,
                                    IS_CLEARED_ALL_SUBJECT,
                                    AUTHORIZE_PERSON_NAME,
                                    AUTHORIZE_REASON,
                                    AUTHORIZE_PERSON,
                                    EXTRA_INFORMATION,
                                    DATE_FORMAT(MSD.REQUEST_DATE, '%Y/%m/%d') AS REQUEST_DATE,
                                    DATE_FORMAT(MSD.APPROVED_DATE, '%Y/%m/%d') AS APPROVED_DATE,
                                    SCCL.CATEGORY_NAME AS CERTIFICATE_TYPE
                                FROM
                                    COE_CERTIFICATE_DISTRIBUTION AS MSD
                                        INNER JOIN
                                    (SELECT 
                                        STUDENT_ID, MAX(ACADEMIC_YEAR) AS MAX_YEAR
                                    FROM
                                        STU_CLASS
                                    WHERE
                                        IS_DELETED != 1
                                    GROUP BY STUDENT_ID) AS SC_MAX ON SC_MAX.STUDENT_ID = MSD.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SC_MAX.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = SC_MAX.MAX_YEAR
                                        AND SC.IS_DELETED != 1
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        INNER JOIN
                                    SUP_CERTIFICATE_LIST AS SS ON SS.CERTIFICATE_ID = MSD.CERTIFICATE_ID
                                        INNER JOIN
                                    SUP_STATUS AS S ON S.STATUS_ID = MSD.STATUS
                                        INNER JOIN
                                    sup_certificate_category_list AS SCCL ON SCCL.CATEGORY_ID=MSD.CERTIFICATE_TYPE
                                WHERE
                                    MSD.IS_DELETED != 1
                                    AND DATE_FORMAT(MSD.APPROVED_DATE, '%Y-%m-%d') BETWEEN ?START_DATE AND ?END_DATE
                                    AND MSD.STATUS in (?STATUS);";
                        break;
                    }
                case AlumnaeSQLCommands.FetchCertificateRequestedListAcademicYearWise:
                    {

                        query = @"SELECT 
                                    SP.FIRST_NAME,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    CC.CLASS_ID,
                                    CP.PROGRAMME_NAME,
                                    CC.CLASS_NAME,
                                    MSD.CERTIFICATE_DISTRIBUTION_ID,
                                    MSD.STUDENT_ID,
                                    MSD.REASON,
                                    MSD.STATUS,
                                    IFNULL(S.STATUS, 'Pending') AS STATUS_NAME,
                                    SS.CERTIFICATE_NAME,
                                    CONTACT_NO,
                                    MONTH_OF_SEMESTER,
                                    YEAR_OF_SEMESTER,
                                    IS_CLEARED_ALL_SUBJECT,
                                    AUTHORIZE_PERSON_NAME,
                                    AUTHORIZE_REASON,
                                    AUTHORIZE_PERSON,
                                    EXTRA_INFORMATION,
                                    DATE_FORMAT(MSD.REQUEST_DATE, '%Y/%m/%d') AS REQUEST_DATE,
                                    DATE_FORMAT(MSD.APPROVED_DATE, '%Y/%m/%d') AS APPROVED_DATE,
                                    SCCL.CATEGORY_NAME AS CERTIFICATE_TYPE
                                FROM
                                    COE_CERTIFICATE_DISTRIBUTION AS MSD
                                        INNER JOIN
                                    (SELECT 
                                        STUDENT_ID, MAX(ACADEMIC_YEAR) AS MAX_YEAR
                                    FROM
                                        STU_CLASS
                                    WHERE
                                        IS_DELETED != 1
                                    GROUP BY STUDENT_ID) AS SC_MAX ON SC_MAX.STUDENT_ID = MSD.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SC_MAX.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = SC_MAX.MAX_YEAR
                                        AND SC.IS_DELETED != 1
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        INNER JOIN
                                    SUP_CERTIFICATE_LIST AS SS ON SS.CERTIFICATE_ID = MSD.CERTIFICATE_ID
                                        INNER JOIN
                                    SUP_STATUS AS S ON S.STATUS_ID = MSD.STATUS
                                        INNER JOIN
                                    sup_certificate_category_list AS SCCL ON SCCL.CATEGORY_ID=MSD.CERTIFICATE_TYPE
                                WHERE
                                    MSD.IS_DELETED != 1
                                    AND SC.ACADEMIC_YEAR IN (?AC_YEAR);";
                        break;
                    }
                case AlumnaeSQLCommands.FetchCertificateLeaveUpdateDate:
                    {
                        query = @"SELECT 
                                       LEAVEREQUEST_ID,
                                       DATE_FORMAT(FROM_DATE, '%d/%m/%Y') AS FROM_DATE,
                                       DATE_FORMAT(TO_DATE, '%d/%m/%Y') AS TO_DATE,
                                       REASON,
                                       DATE_FORMAT(CREATED_DATE, '%d/%m/%Y %h:%i %p') AS CREATED_DATE
                                    FROM
                                        COE_CERTIFICATE_ISSUE_LEAVE
                                    WHERE
                                        IS_DELETED != 1 AND ACADEMIC_YEAR =?AC_YEAR
                                    ORDER BY LEAVEREQUEST_ID DESC;";
                        break;
                    }
                case AlumnaeSQLCommands.DeleteCertificateLeaveUpdate:
                    {
                        query = @" UPDATE COE_CERTIFICATE_ISSUE_LEAVE 
                                        SET 
                                            IS_DELETED = ?IS_DELETED
                                        WHERE
                                            LEAVEREQUEST_ID = ?LEAVEREQUEST_ID";
                        break;
                    }
                case AlumnaeSQLCommands.FetchCertificateLeaveUpdateById:
                    {
                        query = @" SELECT 
                                       LEAVEREQUEST_ID,
                                       DATE_FORMAT(FROM_DATE, '%d/%m/%Y') AS FROM_DATE,
                                       DATE_FORMAT(TO_DATE, '%d/%m/%Y') AS TO_DATE,
                                       REASON,
                                       DATE_FORMAT(CREATED_DATE, '%d/%m/%Y %h:%i %p') AS CREATED_DATE
                                    FROM
                                        COE_CERTIFICATE_ISSUE_LEAVE
                                    WHERE
                                        IS_DELETED != 1 and LEAVEREQUEST_ID=?LEAVEREQUEST_ID
                                    ORDER BY LEAVEREQUEST_ID DESC;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchCertificateCategoryList:
                    {
                        query = @"SELECT 
                                        CATEGORY_ID,
                                        CATEGORY_NAME
                                    FROM
                                        sup_certificate_Category_list
                                    WHERE
                                        IS_DELETED != 1 AND IS_ACTIVE = 1
                                    ORDER BY ORDER_BY;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchStuContactNoForCertificate:
                    {
                        query = @"SELECT 
                                        IFNULL(CCD.CONTACT_NO, spi.STU_MOBILENO) AS CONTACT_NO,
                                        CERTIFICATE_DISTRIBUTION_ID,
                                        IFNULL(CCD.REGISTER_NO, SPI.REGISTER_NO) AS REGISTER_NO
                                    FROM
                                        STU_PERSONAL_INFO AS SPI
                                            LEFT JOIN
                                        COE_CERTIFICATE_DISTRIBUTION AS CCD ON SPI.STUDENT_ID = CCD.STUDENT_ID
                                    WHERE
                                        SPI.STUDENT_ID = ?STUDENT_ID
                                            AND SPI.IS_DELETED != 1;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchAuthorizerList:
                    {
                        query = @"SELECT 
                                        AUTHORIZER_ID, AUTHORIZER_NAME
                                    FROM
                                        sup_certificate_authorize_person
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                
                case AlumnaeSQLCommands.FetcNameRegisterNobyRollNo:
                    {
                        query = @"SELECT 
                                        IFNULL(CCD.CONTACT_NO, spi.STU_MOBILENO) AS CONTACT_NO,
                                        CERTIFICATE_DISTRIBUTION_ID,
                                        IFNULL(CCD.REGISTER_NO, SPI.REGISTER_NO) AS REGISTER_NO,
                                        SPI.FIRST_NAME,
                                        SPI.STUDENT_ID
                                    FROM
                                        STU_PERSONAL_INFO AS SPI
                                            LEFT JOIN
                                        COE_CERTIFICATE_DISTRIBUTION AS CCD ON SPI.STUDENT_ID = CCD.STUDENT_ID
                                    WHERE
                                        SPI.ROLL_NO = ?ROLL_NO
                                            AND SPI.IS_DELETED != 1;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchCertificateRequestedForAdmin:
                    {
                        query = @"SELECT 
                                        MSD.CERTIFICATE_DISTRIBUTION_ID,
                                        MSD.STUDENT_ID,
                                        MSD.STATUS,
                                        spi.FIRST_NAME,
                                        SPI.ROLL_NO,
                                        MSD.REGISTER_NO,
                                        MSD.REASON,
                                        MSD.AUTHORIZE_REASON,
                                        MONTH_OF_SEMESTER,
                                        YEAR_OF_SEMESTER,
                                        MSD.CONTACT_NO,
                                        MSD.AUTHORIZE_PERSON_NAME,
                                        MSD.AUTHORIZE_PERSON,
                                        IFNULL(S.STATUS, 'Pending') AS STATUS_NAME,
                                        SS.CERTIFICATE_NAME,
                                        DATE_FORMAT(MSD.REQUEST_DATE, '%Y/%m/%d') AS REQUEST_DATE,
                                        DATE_FORMAT(MSD.APPROVED_DATE, '%Y/%m/%d') AS APPROVED_DATE,
                                        DATE_FORMAT(MSD.TO_BE_COLLECTED_DATE, '%Y/%m/%d') AS TO_BE_COLLECTED_DATE,
                                        DATE_FORMAT(MSD.ISSUED_DATE, '%Y/%m/%d') AS ISSUED_DATE,
                                        CERTIFICATE_STATUS AS CERTIFICATE_STATUS
                                    FROM
                                        COE_CERTIFICATE_DISTRIBUTION AS MSD
                                            INNER JOIN
                                        SUP_CERTIFICATE_LIST AS SS ON SS.CERTIFICATE_ID = MSD.CERTIFICATE_ID
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = MSD.STUDENT_ID
                                            LEFT JOIN
                                        SUP_STATUS AS S ON S.STATUS_ID = MSD.STATUS
                                    WHERE
                                        MSD.IS_DELETED != 1 AND  ( MSD.STATUS=0 || MSD.STATUS IS NULL);";
                        break;
                    }
                case AlumnaeSQLCommands.FetchCertificateRquestInfoByIdForAdmin:
                    {
                        query = @"SELECT 
                                        MSD.CERTIFICATE_DISTRIBUTION_ID,
                                        MSD.STUDENT_ID,
                                        MSD.REGISTER_NO,
                                        MSD.STATUS,
                                        MSD.REASON,
                                        MSD.CERTIFICATE_TYPE,
                                        MSD.CONTACT_NO,
                                        IFNULL(S.STATUS, 'Pending') AS STATUS_NAME,
                                        SS.CERTIFICATE_NAME,
                                        DATE_FORMAT(MSD.REQUEST_DATE, '%Y/%m/%d') AS REQUEST_DATE,
                                        DATE_FORMAT(MSD.APPROVED_DATE, '%Y/%m/%d') AS APPROVED_DATE,
                                        DATE_FORMAT(MSD.ISSUED_DATE, '%Y/%m/%d') AS ISSUED_DATE,
                                        CERTIFICATE_STATUS AS CERTIFICATE_STATUS,
                                        MSD.MONTH_OF_SEMESTER,
                                        MSD.YEAR_OF_SEMESTER,
                                        MSD.AUTHORIZE_PERSON,
                                        AUTHORIZE_PERSON_NAME,
                                        MSD.AUTHORIZE_REASON,
                                        MSD.CERTIFICATE_ID,
                                        SPI.FIRST_NAME,SPI.ROLL_NO
                                    FROM
                                        COE_CERTIFICATE_DISTRIBUTION AS MSD
                                            INNER JOIN
                                        SUP_CERTIFICATE_LIST AS SS ON SS.CERTIFICATE_ID = MSD.CERTIFICATE_ID
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = MSD.STUDENT_ID 
                                            LEFT JOIN
                                        SUP_STATUS AS S ON S.STATUS_ID = MSD.STATUS
                                    WHERE
                                        MSD.IS_DELETED != 1
                                            AND MSD.CERTIFICATE_DISTRIBUTION_ID = ?CERTIFICATE_DISTRIBUTION_ID;";
                        break;
                    }
                #endregion

                #region 

                case AlumnaeSQLCommands.FetchCertificate:
                    {
                        query = @"   SELECT 
 	                                SCL.CERTIFICATE_ID,
                                    GROUP_CONCAT(SCCL.CATEGORY_NAME) AS CATEGORY_NAME,
                                    SCL.CERTIFICATE_NAME,
                                    SCL.ORDER_BY,
                                    SCL.IS_ACTIVE,
                                    SCL.IS_DELETED,
                                    SCL.CATEGORY_ID
                                FROM
                                    SUP_CERTIFICATE_LIST AS SCL
                                        LEFT JOIN
                                    SUP_CERTIFICATE_CATEGORY_LIST AS SCCL ON FIND_IN_SET(SCCL.CATEGORY_ID, SCL.CATEGORY_ID)
                                WHERE
                                     SCL.IS_DELETED!=1
                                GROUP BY SCL.CERTIFICATE_ID
                                ORDER BY SCL.ORDER_BY;";
                        break;
                    }

                case AlumnaeSQLCommands.UpdateCertificate:
                    {
                        query = @"UPDATE 
                                    SUP_CERTIFICATE_LIST
                                    SET 
                                    CERTIFICATE_NAME=?CERTIFICATE_NAME,
                                    IS_ACTIVE=?IS_ACTIVE,
                                    ORDER_BY=?ORDER_BY,
                                    CATEGORY_ID=?CATEGORY_ID
                                    WHERE CERTIFICATE_ID=?CERTIFICATE_ID
                                    AND IS_DELETED=0;";
                        break;
                    }



                case AlumnaeSQLCommands.FetchCertificateByUserType:

                    query = @"SELECT 
                                            CERTIFICATE_ID,
                                            CERTIFICATE_NAME,
                                            IS_ACTIVE,
                                            IS_DELETED,
                                            ORDER_BY

                                        FROM
                                            SUP_CERTIFICATE_LIST
                                        WHERE
                                            CERTIFICATE_ID = ?CERTIFICATE_ID
                                            AND  CERTIFICATE_NAME=?CERTIFICATE_NAME
                                            AND  IS_ACTIVE=?IS_ACTIVE

                                            AND IS_DELETED = 0;
                                            AND ORDER_BY =? ORDER_BY";

                    break;


                case AlumnaeSQLCommands.AddCertificate:
                    {
                        query = @"INSERT INTO SUP_CERTIFICATE_LIST
                                    (
                                     CERTIFICATE_NAME,
                                            IS_ACTIVE,
                                            ORDER_BY,
                                            CATEGORY_ID)
                                    VALUES
                                    (
                                    ?CERTIFICATE_NAME,
                                    ?IS_ACTIVE,
                                    ?ORDER_BY,
                                    ?CATEGORY_ID);";
                                  
                        break;
                    }

                case AlumnaeSQLCommands.FetchSupCertificatebyId:
                    {
                        query = @"SELECT
                                CERTIFICATE_ID,
								CERTIFICATE_NAME,
                                IS_ACTIVE,
                                IS_DELETED,
                                ORDER_BY,
                                CATEGORY_ID

                                FROM 
                                sup_certificate_list

                                WHERE CERTIFICATE_ID=?CERTIFICATE_ID
                                AND IS_DELETED=0;";
                        break;

                    }

                case AlumnaeSQLCommands.DeleteSupCertificateById:
                    {
                        query = @"UPDATE SUP_CERTIFICATE_LIST SET IS_DELETED=?IS_DELETED WHERE CERTIFICATE_ID=?CERTIFICATE_ID";
                        break;
                    }
                #endregion

                #region certificate category
                case AlumnaeSQLCommands.FetchCategory:
                    {
                        query = @"SELECT 
                                      CATEGORY_ID,
                                      CATEGORY_NAME,
                                      IS_ACTIVE,
                                      IS_DELETED,
                                      ORDER_BY
                                  FROM
                                      sup_certificate_category_list
                                  WHERE
                                     IS_DELETED !=1;";
                        break;
                    }

                case AlumnaeSQLCommands.UpdateCertificateCategory:
                    {
                        query = @"UPDATE 
                                    sup_certificate_category_list
                                    SET 
                                    CATEGORY_NAME=?CATEGORY_NAME,
                                    IS_ACTIVE=?IS_ACTIVE,
                                    ORDER_BY=?ORDER_BY
                                   

                                    WHERE CATEGORY_ID=?CATEGORY_ID
                                    AND IS_DELETED!=1;";
                        break;
                    }

                case AlumnaeSQLCommands.AddCertificateCategory:
                    {
                        query = @"INSERT INTO sup_certificate_category_list
                                    (
                                     CATEGORY_NAME,
                                     IS_ACTIVE,
                                     ORDER_BY)
                                    VALUES
                                    (
                                    ?CATEGORY_NAME,
                                    ?IS_ACTIVE,
                                    ?ORDER_BY );";

                        break;
                    }

                case AlumnaeSQLCommands.FetchSupCertificateCategorybyId:
                    {
                        query = @"SELECT
                                CATEGORY_ID,
								CATEGORY_NAME,
                                IS_ACTIVE,
                                IS_DELETED,
                                ORDER_BY

                                FROM 
                                sup_certificate_category_list

                                WHERE CATEGORY_ID=?CATEGORY_ID
                                AND IS_DELETED !=1;";
                        break;

                    }


                case AlumnaeSQLCommands.DeleteSupCertificateCategoryById:
                    {
                        query = @"UPDATE sup_certificate_category_list 
                                SET 
                                IS_DELETED=?IS_DELETED
                                WHERE 
                               CATEGORY_ID=?CATEGORY_ID";
                        break;
                    }

                case AlumnaeSQLCommands.FetchCertificateByCategoryId:
                    {
                        query = @" SELECT 
                                    CERTIFICATE_ID,
                                    CERTIFICATE_NAME
                                FROM
                                    SUP_CERTIFICATE_LIST
                                WHERE
                                    FIND_IN_SET(?CATEGORY_ID, CATEGORY_ID)
                                    AND IS_ACTIVE=1
                                    AND IS_DELETED=0
                                ORDER BY ORDER_BY;";
                        break;
                    }
                #endregion

                case AlumnaeSQLCommands.Is_eligibleforconvocation:
                    {
                        query = @"
                                    SELECT 
                                        STUDENT_ID, 
                                        CE.ACADEMIC_YEAR, 
                                        CE.IS_ACTIVE, 
                                        IS_BLOCKED,
                                        DATE_FORMAT(DATE_FROM, '%Y-%m-%d %H:%i:%s') AS DATE_FROM,
                                        DATE_FORMAT(TO_DATE, '%Y-%m-%d %H:%i:%s') AS TO_DATE,
                                        SETTING_NAME,
                                        REGISTRATION_AMOUNT,
                                        CS.SETTING_ID,IS_PAID,DOWNLOAD_AUTH_TYPE,CONVOCATION_NO,CONVOCATION_DATE,YEAR_OF_PASSING,CE.APPROVAL
                                    FROM
                                        CONVOCATION_ELEGIBILITY AS CE
                                    INNER JOIN
                                        CONVOCATION_SETTING AS CS ON CE.SETTING_ID = CS.SETTING_ID
                                        AND CS.IS_DELETED != 1
                                        AND CS.IS_ACTIVE = 1
                                    WHERE
                                        STUDENT_ID = ?STUDENT_ID
                                        AND CE.IS_DELETED != 1
                                        AND CE.IS_ACTIVE = 1;
                                 ";
                        break;
                    }
                case AlumnaeSQLCommands.Getisalreadysubmitted:
                    {
                        query = @"
                                 SELECT 
                                     STUDENT_ID,
                                     QUALIFICATION,
                                     PHONE_NO,
                                     EMAIL,
                                     AADHAR_NO,
                                     ACADEMIC_YEAR
                                 FROM
                                     ACF_STU_INFO
                                 WHERE
                                     STUDENT_ID = ?STUDENT_ID AND IS_DELETED != 1;";
                        break;
                    }
                case AlumnaeSQLCommands.Getisalreadysubmittedacf:
                    {
                        query = @"
                                 SELECT 
                                     STUDENT_ID,
                                     QUESTION_ID,
                                     ANSWER_ID,
                                     SHORT_ANSWER,
                                     ACADEMIC_YEAR,
                                     ACADEMIC_YEAR
                                 FROM
                                     ACF_ENTRY
                                 WHERE
                                     STUDENT_ID = ?STUDENT_ID AND IS_DELETED != 1;
                                          ";
                        break;
                    }
                case AlumnaeSQLCommands.Getisalreadysubmittedap:
                    {
                        query = @"
                                 SELECT 
                                     STUDENT_ID,
                                     APSTATUS,
                                     COURSE_NAME
                                 FROM
                                     ALUMNAE_PROGRESSION
                                 WHERE
                                     STUDENT_ID = ?STUDENT_ID AND IS_DELETED != 1;
                                          ";
                        break;
                    }
                case AlumnaeSQLCommands.GetProgression:
                    {
                        query = @"
                                   SELECT 
                                        STUDENT_ID,
                                        SAS.STATUS as CSTATUS,
                                        SAS.AP_STATUS AS APSTATUS,
                                        COURSE_NAME,
                                        INSTITUTION_COMPANY_NAME,
                                        INSTITUTION_COMPANY_ADDRESS,
                                        INSTITUTION_COMPANY_NO,
                                        INSTITUTION_COMPANY_EMAIL,
                                        `FILE`,
                                        SAP.INCOME,
                                        EXAM_CLEARED,
                                        PASSED_YEAR,
                                        EXAM_NAME,
                                        CLEARED_FILE,
                                        SUGGESTIONS,
                                        SPECIFY,
                                        SAC.COLLOBORATION as COLLOBRATION,
                                        DURATION
                                    FROM
                                        ALUMNAE_PROGRESSION AS AP
                                            INNER JOIN
                                        SUP_AP_STATUS AS SAS ON AP.APSTATUS = SAS.AP_STATUS
                                            INNER JOIN
                                        SUP_AP_COLLABORATION AS SAC ON AP.COLLOBRATION = SAC.COLLOBORATION_ID
                                            LEFT JOIN 
                                        SUP_AP_INCOME AS SAP ON SAP.INCOME_ID = AP.INCOME
                                     WHERE AP.STUDENT_ID=?STUDENT_ID AND AP.IS_DELETED !=1;
                                 ";
                        break;
                    }
                case AlumnaeSQLCommands.Getprofileconvocation:
                    {
                        query = @"
                                   SELECT 
                                       SP.REGISTER_NO,
                                       SP.DATE_OF_BIRTH,
                                       UPPER(TRIM(SP.FIRST_NAME)) AS FIRST_NAME,
                                       CP.PROGRAMME_DESCRIPTION AS PROGRAMME_NAME,
                                       CONCAT(BEGIN_YEAR, '-', EXPIRED_YEAR) AS DURATION,
                                       CD.DEPARTMENT,
                                       ACF.QUALIFICATION,
                                       ACF.PHONE_NO,
                                       ACF.EMAIL,
                                       ACF.AADHAR_NO								
                                   FROM
                                       STU_CLASS AS SC
                                           INNER JOIN
                                       STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                           INNER JOIN
                                       CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                           INNER JOIN
                                       CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                           INNER JOIN
                                       CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = CP.DEPARTMENT
                                           INNER JOIN
                                       CP_CLASSES AS CPC ON CPC.CLASS_ID = SC.CLASS_ID
                                           INNER JOIN
                                       SUP_CLASS_YEAR AS SCY ON SCY.CLASS_YEAR_ID = CPC.CLASS_YEAR
                                           INNER JOIN
                                       ACADEMIC_YEAR AS AY ON AY.AC_YEAR = SC.ACADEMIC_YEAR
                                           INNER JOIN
                                       SUP_BATCH_DURATION AS SB ON SB.BATCH_ID = SC.BATCH
                                           AND SB.APPLICATION_TYPE_ID = CPG.APPTYPE_ID
                                           INNER JOIN 
                                       ACF_STU_INFO AS ACF ON ACF.STUDENT_ID = SP.STUDENT_ID
                                   WHERE
                                       SP.STUDENT_ID = ?STUDENT_ID AND ACF.IS_DELETED != 1
                                           AND SC.IS_DELETED != 1
                                           AND SC.ACADEMIC_YEAR = (
                                               SELECT MAX(ACADEMIC_YEAR)
                                               FROM STU_CLASS
                                               WHERE STUDENT_ID = ?STUDENT_ID
                                                   AND IS_DELETED != 1
                                           )
                                   ORDER BY PROGRAMME_NAME;
                                                                    ";
                        break;
                    }
                case AlumnaeSQLCommands.Fetchconvocationremarkstostu:
                    {
                        query = @"SELECT 
                                      REMARK_ID, REMARK, IS_ACTIVE
                                  FROM
                                      sup_convocation_remarks
                                  WHERE
                                      REMARK_ID IN (?REMARK_ID);";
                        break;
                    }
                case AlumnaeSQLCommands.savealumnaefeedbackpersonal:
                    {
                        query = @"
                                 INSERT 
                                     INTO ACF_STU_INFO
                                       (STUDENT_ID, 
                                       QUALIFICATION,
                                       PHONE_NO,
                                       EMAIL,
                                       AADHAR_NO,
                                       ACADEMIC_YEAR,
                                       PLACEMENT_HELP
                                       )
                                    VALUES(?STUDENT_ID,
                                       ?QUALIFICATION,
                                       ?PHONE_NO,
                                       ?EMAIL,
                                       ?AADHAR_NO,
                                       ?ACADEMIC_YEAR,?PLACEMENT_HELP); 
                                          ";
                        break;
                    }
                case AlumnaeSQLCommands.updateStuMobileandEmailinStuPersonal:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO SET STU_MOBILENO=?PHONE_NO, STU_EMAILID =?EMAIL WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case AlumnaeSQLCommands.UPDATEREMARKSFORPROFILE:
                    {
                        query = @"
                                   UPDATE convocation_elegibility SET PROFILE_REMARKS = ?REMARK WHERE STUDENT_ID = ?STUDENT_ID;
                                 ";
                        break;
                    }
                case AlumnaeSQLCommands.Fetchquestionsforconvocation:
                    {
                        query = @" SELECT 
                                       ACF_ID,
                                       QUESTION,
                                       ACADEMIC_YEAR,
                                       `ORDER`,
                                       IS_ACTIVE,
                                       IS_SHORT_ANSWER
                                   FROM
                                       ACF_QUESTIONS
                                   WHERE
                                       IS_ACTIVE = 1 AND IS_DELETED != 1;";
                        break;
                    }
                case AlumnaeSQLCommands.Fetchobjectivesforconvocation:
                    {
                        query = @" SELECT 
                                       OBJECTIVE_ID, OBJECTIVE_TEXT, `ORDER`, IS_ACTIVE, IS_DELETED
                                   FROM
                                       ACF_OBJECTIVES
                                   WHERE
                                       IS_ACTIVE = 1 AND IS_DELETED != 1;";
                        break;
                    }
                case AlumnaeSQLCommands.savefeedbackcurriculum:
                    {
                        query = @" 
                                  INSERT 
                                       INTO ACF_ENTRY
                                       (STUDENT_ID, 
                                       QUESTION_ID,
                                       ANSWER_ID,
                                       SHORT_ANSWER,
                                       ACADEMIC_YEAR
                                       )
                                       VALUES(?STUDENT_ID,
                                       ?QUESTION_ID,
                                       ?ANSWER_ID,
                                       ?SHORT_ANSWER,
                                       ?ACADEMIC_YEAR);
                                  ";
                        break;
                    }
                case AlumnaeSQLCommands.saveacfpersonal:
                    {
                        query = @" 
                                  INSERT 
                                       INTO ACF_STU_INFO
                                       (STUDENT_ID, 
                                       QUALIFICATION,
                                       PHONE_NO,
                                       EMAIL,
                                       AADHAR_NO,
                                       ACADEMIC_YEAR
                                       )
                                       VALUES(?STUDENT_ID,
                                       ?QUALIFICATION,
                                       ?PHONE_NO,
                                       ?EMAIL,
                                       ?AADHAR_NO,
                                       ?ACADEMIC_YEAR);
                                  ";
                        break;
                    }
                case AlumnaeSQLCommands.UPDATEREMARKSFORFEEDBACK:
                    {
                        query = @"
                                   UPDATE convocation_elegibility SET FB_REMARKS = ?REMARK WHERE STUDENT_ID = ?STUDENT_ID;
                                 ";
                        break;
                    }
                case AlumnaeSQLCommands.fetchapincome:
                    {
                        query = @" 
                                 SELECT 
                                     INCOME_ID, INCOME, IS_ACTIVE
                                 FROM
                                     SUP_AP_INCOME
                                 WHERE
                                     IS_ACTIVE = 1 AND IS_DELETED != 1; 
                                  ";
                        break;
                    }
                case AlumnaeSQLCommands.fetchapstatus:
                    {
                        query = @" 
                                  SELECT 
                                      AP_STATUS, STATUS, IS_ACTIVE
                                  FROM
                                      SUP_AP_STATUS
                                  WHERE
                                      IS_ACTIVE = 1 AND IS_DELETED != 1; 
                                  ";
                        break;
                    }
                case AlumnaeSQLCommands.fetchapcolloboration:
                    {
                        query = @"
                                 SELECT 
                                     COLLOBORATION_ID, COLLOBORATION, IS_ACTIVE
                                 FROM
                                     SUP_AP_COLLABORATION
                                 WHERE
                                     IS_ACTIVE = 1 AND IS_DELETED != 1; 
                                  ";
                        break;
                    }
                case AlumnaeSQLCommands.UPDATEREMARKS:
                    {
                        query = @"
                                   UPDATE convocation_elegibility SET AP_REMARKS = ?REMARK WHERE STUDENT_ID = ?STUDENT_ID;
                                 ";
                        break;
                    }
                case AlumnaeSQLCommands.DELETEPROGRESSION:
                    {
                        query = @"
                                   UPDATE alumnae_progression SET IS_DELETED = 1 WHERE STUDENT_ID= ?STUDENT_ID;
                                 ";
                        break;
                    }
                case AlumnaeSQLCommands.DELETEACFPERSONAL:
                    {
                        query = @"
                                   UPDATE acf_stu_info SET IS_DELETED = 1 WHERE STUDENT_ID= ?STUDENT_ID;
                                 ";
                        break;
                    }
                case AlumnaeSQLCommands.DELETEFEEDBACK:
                    {
                        query = @"
                                    UPDATE acf_entry SET IS_DELETED = 1 WHERE STUDENT_ID= ?STUDENT_ID;
                                 ";
                        break;
                    }
                case AlumnaeSQLCommands.savealumnaeprogression:
                    {
                        query = @"
                                 INSERT 
                                    INTO ALUMNAE_PROGRESSION
                                      (STUDENT_ID, 
                                      APSTATUS,
                                      COURSE_NAME,
                                      INSTITUTION_COMPANY_NAME,
                                      INSTITUTION_COMPANY_ADDRESS,
                                      INSTITUTION_COMPANY_NO,
                                      INSTITUTION_COMPANY_EMAIL,
                                      FILE,
                                      INCOME,
                                      EXAM_CLEARED,
                                      PASSED_YEAR,
                                      EXAM_NAME,
                                      CLEARED_FILE,
                                      COLLOBRATION,
                                      DURATION,
                                      SPECIFY,
                                      SUGGESTIONS,
                                      ACADEMIC_YEAR,EINCOME,DESIGNATION
                                      )
                                   VALUES(?STUDENT_ID,
                                      ?APSTATUS,
                                      ?COURSE_NAME,
                                      ?INSTITUTION_COMPANY_NAME,
                                      ?INSTITUTION_COMPANY_ADDRESS,
                                      ?INSTITUTION_COMPANY_NO,
                                      ?INSTITUTION_COMPANY_EMAIL,
                                      ?FILE,
                                      ?INCOME,
                                      ?EXAM_CLEARED,
                                      ?PASSED_YEAR,
                                      ?EXAM_NAME,
                                      ?CLEARED_FILE,
                                      ?COLLOBRATION,
                                      ?DURATION,
                                      ?SPECIFY,
                                      ?SUGGESTIONS,
                                      ?ACADEMIC_YEAR,?EINCOME,?DESIGNATION); 
                                          ";
                        break;
                    }
                case AlumnaeSQLCommands.Fetchconvocationsetting:
                    {
                        query = @"SELECT 
                                      SETTING_ID,
                                      SETTING_NAME,
                                      BATCH,
                                      ACADEMIC_YEAR,
                                      DATE_FROM,
                                      TO_DATE,
                                      IS_ACTIVE
                                  FROM
                                      CONVOCATION_SETTING
                                  WHERE
                                      IS_DELETED != 1 AND IS_ACTIVE =1;";
                        break;
                    }
                case AlumnaeSQLCommands.Fetchconvocationremarks:
                    {
                        query = @"SELECT 
                                      REMARK_ID,
                                      REMARK,
                                      IS_ACTIVE
                                  FROM
                                      sup_convocation_remarks
                                  WHERE
                                      IS_DELETED != 1 AND IS_ACTIVE =1;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchProgrammeWiseconvocationStudent:
                    {
                        query = @"SELECT 
                                    SP.STUDENT_ID,
                                    SP.FIRST_NAME,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH, '%d-%m-%Y') AS DATE_OF_BIRTH,
                                    PROGRAMME_DESCRIPTION AS PROGRAMME_NAME,
                                    REGISTER_NO,
                                    ROLL_NO,
                                    SB.BATCH,
                                    CE.APPROVAL,
                                    DEBIT
                                FROM
                                    STU_PERSONAL_INFO AS SP
                                        INNER JOIN
                                    CONVOCATION_ELEGIBILITY AS CE ON SP.STUDENT_ID = CE.STUDENT_ID
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                        INNER JOIN
                                    CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                        INNER JOIN
                                    SUP_BATCHES AS SB ON SP.BATCH = SB.BATCH_ID
                                        LEFT JOIN
                                    FEE_FREQUENCY_FEE_MODE AS FFM ON FFM.FEE_MODE = ?FEE_MODE
                                        LEFT JOIN
                                    FEE_STUDENT_ACCOUNT AS FS ON SP.STUDENT_ID = FS.STUDENT_ID
                                        AND FS.FREQUENCY_ID = ffm.FREQUENCY_ID
                                        AND DEBIT IS NOT NULL
                                WHERE
                                    CPG.PROGRAMME_GROUP_ID = ?PROGRAMME_ID
                                        AND CE.SETTING_ID = ?SETTING_ID
                                        AND SP.IS_DELETED != 1
                                        AND CE.IS_DELETED != 1;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchStudentFEEDBACK:
                    {
                        query = @"
                                   SELECT 
                                       ACF.STUDENT_ID,
                                       AQ.QUESTION,
                                       CASE 
                                           WHEN AQ.IS_SHORT_ANSWER = 1 THEN ACF.SHORT_ANSWER
                                           ELSE AO.OBJECTIVE_TEXT 
                                       END AS ANSWER
                                   FROM
                                       ACF_ENTRY AS ACF
                                           LEFT JOIN
                                       ACF_QUESTIONS AS AQ ON AQ.ACF_ID = ACF.QUESTION_ID
                                           LEFT JOIN
                                       ACF_OBJECTIVES AS AO ON AO.OBJECTIVE_ID = ACF.ANSWER_ID
                                   WHERE
                                       ACF.STUDENT_ID = ?STUDENT_ID
                                          AND ACF.IS_DELETED != 1;
                                 ";
                        break;
                    }
                case AlumnaeSQLCommands.Getisalreadypaid:
                    {
                        query = @"
                                SELECT 
                                STUDENT_ID,HEAD                                                       
                                FROM
                                    FEE_STUDENT_ACCOUNT
                                WHERE
                                    DEBIT IS NOT NULL AND HEAD = 94
                                    AND FREQUENCY_ID = 47
                                    AND STUDENT_ID = ?STUDENT_ID AND IS_DELETED != 1;
                                          ";
                        break;
                    }
                case AlumnaeSQLCommands.UPDATEAPPROVAL:
                    {
                        query = @"
                                   UPDATE convocation_elegibility SET APPROVAL = ?APPROVAL WHERE STUDENT_ID = ?STUDENT_ID;
                                 ";
                        break;
                    }
                case AlumnaeSQLCommands.FetchProgrammelistfordqac:
                    {
                        query = @"SELECT 
                                      CPG.PROGRAMME_GROUP_ID AS PROGRAMME_ID,
                                      CONCAT(CP.PROGRAMME_DESCRIPTION,
                                              ' - ',
                                              SPP.PROGRAMME_MODE,
                                              ' - ',
                                              SS.DESCRIPTION) AS PROGRAMME_DESCRIPTION
                                  FROM
                                      cp_programme_group AS CPG
                                          INNER JOIN
                                      cp_programme AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                          INNER JOIN
                                      sup_programme_mode AS spp ON spp.PROGRAMME_MODE_ID = CPG.PROGRAMME_MODE
                                          INNER JOIN
                                      sup_shift AS SS ON SS.SHIFT_ID = CPG.SHIFT
                                 AND CPG.IS_DELETED != 1;";
                        break;
                    }
                case AlumnaeSQLCommands.Fetchconvocationdataforstatsignorearrear:
                    {
                        query = @"SELECT 
                                      SP.STUDENT_ID,
                                      SP.FIRST_NAME,
                                      IF(SAP.STATUS IS NULL OR SAP.STATUS = '',
                                          'Not Filled',
                                          SAP.STATUS) AS STATUS,
                                      SB.BATCH,
                                      CONCAT(CP.PROGRAMME_DESCRIPTION,
                                              ' - ',
                                              SPP.PROGRAMME_MODE,
                                              ' - ',
                                              SS.DESCRIPTION) AS PROGRAMME_DESCRIPTION,
                                      SAAP.APPLICATION_TYPE,SAP.ORDER,CS.SETTING_NAME
                                  FROM
                                      CONVOCATION_ELEGIBILITY AS CE
                                          INNER JOIN
                                      STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CE.STUDENT_ID
                                          LEFT JOIN
                                      ALUMNAE_PROGRESSION AS AP ON AP.STUDENT_ID = CE.STUDENT_ID
                                          AND AP.IS_DELETED != 1
                                          LEFT JOIN
                                      SUP_AP_STATUS AS SAP ON SAP.AP_STATUS = AP.APSTATUS
                                          INNER JOIN
                                      CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                          INNER JOIN
                                      CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                          INNER JOIN
                                      SUP_PROGRAMME_MODE AS SPP ON SPP.PROGRAMME_MODE_ID = CPG.PROGRAMME_MODE
                                          INNER JOIN
                                      SUP_SHIFT AS SS ON SS.SHIFT_ID = CPG.SHIFT
                                          INNER JOIN
                                      SUP_APPLICATION_TYPE AS SAAP ON SAAP.APPLICATION_TYPE_ID = CPG.APPTYPE_ID
                                          INNER JOIN
                                      SUP_BATCHES AS SB ON SP.BATCH = SB.BATCH_ID
                                     INNER JOIN CONVOCATION_SETTING AS CS ON CS.SETTING_ID = CE.SETTING_ID
                                  WHERE
                                      CE.SETTING_ID = ?SETTING_ID AND SP.IS_DELETED != 1
                                          AND CE.IS_ACTIVE = 1
                                          AND CPG.APPTYPE_ID = 1
                                          AND SB.BATCH = (SELECT 
                                              MAX(SB.BATCH) AS MAX_BATCH
                                          FROM
                                              CONVOCATION_ELEGIBILITY AS CE
                                                  INNER JOIN
                                              STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CE.STUDENT_ID
                                                  INNER JOIN
                                              CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                                  INNER JOIN
                                              CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                                  INNER JOIN
                                              SUP_APPLICATION_TYPE AS SAAP ON SAAP.APPLICATION_TYPE_ID = CPG.APPTYPE_ID
                                                  INNER JOIN
                                              SUP_BATCHES AS SB ON SP.BATCH = SB.BATCH_ID
                                          WHERE
                                              CE.SETTING_ID = ?SETTING_ID AND CPG.APPTYPE_ID = 1) 
                                  UNION SELECT 
                                      SP.STUDENT_ID,
                                      SP.FIRST_NAME,
                                      IF(SAP.STATUS IS NULL OR SAP.STATUS = '',
                                          'Not Filled',
                                          SAP.STATUS) AS STATUS,
                                      SB.BATCH,
                                      CONCAT(CP.PROGRAMME_DESCRIPTION,
                                              ' - ',
                                              SPP.PROGRAMME_MODE,
                                              ' - ',
                                              SS.DESCRIPTION) AS PROGRAMME_DESCRIPTION,
                                      SAAP.APPLICATION_TYPE,SAP.ORDER,CS.SETTING_NAME
                                  FROM
                                      CONVOCATION_ELEGIBILITY AS CE
                                          INNER JOIN
                                      STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CE.STUDENT_ID
                                          LEFT JOIN
                                      ALUMNAE_PROGRESSION AS AP ON AP.STUDENT_ID = CE.STUDENT_ID
                                          AND AP.IS_DELETED != 1
                                          LEFT JOIN
                                      SUP_AP_STATUS AS SAP ON SAP.AP_STATUS = AP.APSTATUS
                                          INNER JOIN
                                      CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                          INNER JOIN
                                      CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                          INNER JOIN
                                      SUP_PROGRAMME_MODE AS SPP ON SPP.PROGRAMME_MODE_ID = CPG.PROGRAMME_MODE
                                          INNER JOIN
                                      SUP_SHIFT AS SS ON SS.SHIFT_ID = CPG.SHIFT
                                          INNER JOIN
                                      SUP_APPLICATION_TYPE AS SAAP ON SAAP.APPLICATION_TYPE_ID = CPG.APPTYPE_ID
                                          INNER JOIN
                                      SUP_BATCHES AS SB ON SP.BATCH = SB.BATCH_ID
                                     INNER JOIN CONVOCATION_SETTING AS CS ON CS.SETTING_ID = CE.SETTING_ID
                                  WHERE
                                      CE.SETTING_ID = ?SETTING_ID AND SP.IS_DELETED != 1
                                          AND CE.IS_ACTIVE = 1
                                          AND CPG.APPTYPE_ID = 2
                                          AND SB.BATCH = (SELECT 
                                              MAX(SB.BATCH) AS MAX_BATCH
                                          FROM
                                              CONVOCATION_ELEGIBILITY AS CE
                                                  INNER JOIN
                                              STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CE.STUDENT_ID
                                                  INNER JOIN
                                              CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                                  INNER JOIN
                                              CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                                  INNER JOIN
                                              SUP_APPLICATION_TYPE AS SAAP ON SAAP.APPLICATION_TYPE_ID = CPG.APPTYPE_ID
                                                  INNER JOIN
                                              SUP_BATCHES AS SB ON SP.BATCH = SB.BATCH_ID
                                          WHERE
                                              CE.SETTING_ID = ?SETTING_ID AND CPG.APPTYPE_ID = 2);
                                  ";
                        break;
                    }
                case AlumnaeSQLCommands.Fetchconvocationdataforstats:
                    {
                        query = @"SELECT 
                                     SP.STUDENT_ID,
                                     SP.FIRST_NAME,
                                     IF(SAP.STATUS IS NULL OR SAP.STATUS = '',
                                        'Not Filled',
                                        SAP.STATUS) AS STATUS,
                                     SB.BATCH,
                                     CONCAT(CP.PROGRAMME_DESCRIPTION,
                                            ' - ',
                                            SPP.PROGRAMME_MODE,
                                            ' - ',
                                            SS.DESCRIPTION) AS PROGRAMME_DESCRIPTION,
                                     SAAP.APPLICATION_TYPE,SAP.ORDER,CS.SETTING_NAME
                                 FROM
                                     CONVOCATION_ELEGIBILITY AS CE
                                         INNER JOIN
                                     STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CE.STUDENT_ID
                                         LEFT JOIN
                                     ALUMNAE_PROGRESSION AS AP ON AP.STUDENT_ID = CE.STUDENT_ID
                                         AND AP.IS_DELETED != 1
                                         LEFT JOIN
                                     SUP_AP_STATUS AS SAP ON SAP.AP_STATUS = AP.APSTATUS
                                         INNER JOIN
                                     CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                         INNER JOIN
                                     CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                         INNER JOIN
                                     SUP_PROGRAMME_MODE AS SPP ON SPP.PROGRAMME_MODE_ID = CPG.PROGRAMME_MODE
                                         INNER JOIN
                                     SUP_SHIFT AS SS ON SS.SHIFT_ID = CPG.SHIFT
                                         INNER JOIN
                                     SUP_APPLICATION_TYPE AS SAAP ON SAAP.APPLICATION_TYPE_ID = CPG.APPTYPE_ID
                                         INNER JOIN
                                     SUP_BATCHES AS SB ON SP.BATCH = SB.BATCH_ID
                                     INNER JOIN CONVOCATION_SETTING AS CS ON CS.SETTING_ID = CE.SETTING_ID
                                 WHERE
                                     CE.SETTING_ID = ?SETTING_ID
                                     AND SP.IS_DELETED != 1
                                     AND CE.IS_ACTIVE = 1;";
                        break;
                    }
                case AlumnaeSQLCommands.Fetchconvocationinforprint:
                    {
                        query = @"
                                    SELECT 
                                        SP.STUDENT_ID,SP.PHOTO,
                                        UPPER(TRIM(SP.FIRST_NAME)) AS FIRST_NAME,
                                        DATE_FORMAT(SP.DATE_OF_BIRTH, '%d-%m-%Y') as DATE_OF_BIRTH,
                                        PROGRAMME_DESCRIPTION as PROGRAMME_NAME,
                                        CD.DEPARTMENT,
                                        REGISTER_NO,
                                        ROLL_NO,
                                        SAS.STATUS as CSTATUS,
                                        COURSE_NAME,
                                        INSTITUTION_COMPANY_NAME,
                                        INSTITUTION_COMPANY_ADDRESS,
                                        INSTITUTION_COMPANY_NO,
                                        INSTITUTION_COMPANY_EMAIL,
                                        `FILE`,
                                        SAP.INCOME,
                                        EXAM_CLEARED,
                                        PASSED_YEAR,
                                        EXAM_NAME,
                                        CLEARED_FILE,
                                        SUGGESTIONS,
                                        SPECIFY,
                                        SAC.COLLOBORATION as COLLOBRATION,
                                        AP.DURATION,
                                   	 ACF.QUALIFICATION,
                                        ACF.PHONE_NO,
                                   	 ACF.EMAIL,
                                        ACF.AADHAR_NO,AP.APSTATUS
                                    FROM
                                        STU_PERSONAL_INFO AS SP
                                            INNER JOIN
                                        CONVOCATION_ELEGIBILITY AS CE ON SP.STUDENT_ID = CE.STUDENT_ID AND CE.SETTING_ID=?SETTING_ID AND CE.IS_DELETED !=1
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                         INNER JOIN
                                       CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = CP.DEPARTMENT
                                        LEFT JOIN
                                        ALUMNAE_PROGRESSION AS AP ON CE.STUDENT_ID = AP.STUDENT_ID AND  AP.IS_DELETED !=1
                                   	 LEFT JOIN
                                        SUP_AP_STATUS AS SAS ON AP.APSTATUS = SAS.AP_STATUS
                                   	 LEFT JOIN
                                        SUP_AP_COLLABORATION AS SAC ON AP.COLLOBRATION = SAC.COLLOBORATION_ID
                                   	 LEFT JOIN 
                                        SUP_AP_INCOME AS SAP ON SAP.INCOME_ID = AP.INCOME    
                                        LEFT JOIN 
                                       ACF_STU_INFO AS ACF ON ACF.STUDENT_ID = CE.STUDENT_ID AND ACF.IS_DELETED !=1
                                    WHERE
                                        CPG.PROGRAMME_GROUP_ID IN(?PROGRAMME_GROUP_ID)
                                            AND SP.IS_DELETED != 1 AND CE.IS_ACTIVE=1
                                            ORDER BY PROGRAMME_NAME,REGISTER_NO;
                                  ";
                        break;
                    }
                case AlumnaeSQLCommands.Fetchconvocationinforprintfeedback:
                    {
                        query = @"
                                    SELECT 
                                        ACF.STUDENT_ID,
                                        AQ.QUESTION,
                                        CASE 
                                            WHEN AQ.IS_SHORT_ANSWER = 1 THEN ACF.SHORT_ANSWER
                                            ELSE AO.OBJECTIVE_TEXT 
                                        END AS ANSWER,AQ.ORDER
                                     FROM
                                         stu_personal_info AS sp
                                             INNER JOIN
                                         CONVOCATION_ELEGIBILITY AS ce ON SP.STUDENT_ID = CE.STUDENT_ID AND CE.SETTING_ID=?SETTING_ID AND CE.IS_DELETED !=1
                                             INNER JOIN
                                         cp_programme_group AS CPG ON CPG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                             INNER JOIN
                                         cp_programme AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                             INNER JOIN
                                         CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = CP.DEPARTMENT
                                             LEFT JOIN
                                         ACF_ENTRY AS ACF ON ACF.STUDENT_ID = CE.STUDENT_ID AND ACF.IS_DELETED !=1
                                            LEFT JOIN
                                         ACF_QUESTIONS AS AQ ON AQ.ACF_ID = ACF.QUESTION_ID
                                            LEFT JOIN
                                         ACF_OBJECTIVES AS AO ON AO.OBJECTIVE_ID = ACF.ANSWER_ID
                                     WHERE
                                         CPG.PROGRAMME_GROUP_ID IN(?PROGRAMME_GROUP_ID)
                                             AND SP.IS_DELETED != 1 AND CE.IS_ACTIVE=1
                                             ORDER BY PROGRAMME_NAME,REGISTER_NO,AQ.ORDER; 
                                  ";
                        break;


                    }
                case AlumnaeSQLCommands.UpdateAlumniDataCollectionInfo:
                    {
                        query = @"UPDATE ALUMNI_DATA_COLLECTION 
                    SET SALUTATION = ?SALUTATION, 
                        NAME = ?NAME, 
                        PROGRAMME = ?PROGRAMME, 
                        BRANCH_UG = ?BRANCH_UG, 
                        BRANCH_PG = ?BRANCH_PG, 
                        BRANCH_MPHIL = ?BRANCH_MPHIL, 
                        BRANCH_PHD = ?BRANCH_PHD, 
                        BATCH = ?BATCH, 
                        STREAM = ?STREAM, 
                        PRESENT_STATUS = ?PRESENT_STATUS, 
                        DESIGNATION = ?DESIGNATION, 
                        COMPANY = ?COMPANY, 
                        PERMANENT_ADDRESS = ?PERMANENT_ADDRESS, 
                        COMMUNICATION_ADDRESS = ?COMMUNICATION_ADDRESS, 
                        MOBILE_NO = ?MOBILE_NO, 
                        EMAIL_ID = ?EMAIL_ID, 
                        OTHER_DETAILS = ?OTHER_DETAILS, 
                        DATE_OF_BIRTH = ?DATE_OF_BIRTH, 
                        PHOTO = ?PHOTO, 
                        ACADEMIC_YEAR = ?AC_YEAR
                  WHERE ROLL_NO = ?ROLL_NO;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchAllAlumniDataCollectionforHOD:
                    {
                        query = @"SELECT 
                                  AC.SALUTATION,
                                  AC.NAME,
                                  AC.ROLL_NO,
                                  IFNULL(CP_PROG.PROGRAMME_NAME, AC.PROGRAMME) AS PROGRAMME,
                                  AC.BRANCH_UG,
                                  AC.BRANCH_PG,
                                  AC.BRANCH_MPHIL,
                                  AC.BRANCH_PHD,
                                  AC.BATCH,
                                  IFNULL(SPS.SHIFT_NAME, AC.STREAM) AS STREAM,
                                  AC.PRESENT_STATUS,
                                  AC.DESIGNATION,
                                  AC.COMPANY,
                                  AC.PERMANENT_ADDRESS,
                                  AC.COMMUNICATION_ADDRESS,
                                  AC.MOBILE_NO,
                                  AC.EMAIL_ID,
                                  AC.OTHER_DETAILS,
                                  IFNULL(DATE_FORMAT(AC.DATE_OF_BIRTH, '%d-%m-%Y'), AC.DATE_OF_BIRTH) AS DATE_OF_BIRTH,
                                  AC.PHOTO
                              FROM ALUMNI_DATA_COLLECTION AS AC
                              INNER JOIN CP_DEPARTMENT AS CP 
                                  ON cp.DEPARTMENT LIKE CONCAT('%', ac.BRANCH_UG, '%')
                              INNER JOIN STF_PERSONAL_INFO AS SP ON SP.DEPARTMENT=CP.DEPARTMENT_ID
                              LEFT JOIN CP_PROGRAMME_GROUP CPG ON CPG.PROGRAMME_GROUP_ID = AC.PROGRAMME
                              LEFT JOIN CP_PROGRAMME CP_PROG ON CP_PROG.PROGRAMME_ID = CPG.PROGRAMME_ID
                              LEFT JOIN SUP_SHIFT SPS ON SPS.SHIFT_ID = AC.STREAM
                              WHERE SP.STAFF_ID=?STAFF_ID
                              ORDER BY ac.CREATED_DATE DESC;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchProgrammelistfordqacforHOD:
                    {
                        query = @"SELECT 
                                      CPG.PROGRAMME_GROUP_ID AS PROGRAMME_ID,
                                      CONCAT(CP.PROGRAMME_DESCRIPTION,
                                              ' - ',
                                              SPP.PROGRAMME_MODE,
                                              ' - ',
                                              SS.DESCRIPTION) AS PROGRAMME_DESCRIPTION
                                  FROM
                                      CP_PROGRAMME_GROUP AS CPG
                                          INNER JOIN
                                      CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                          INNER JOIN
                                      SUP_PROGRAMME_MODE AS SPP ON SPP.PROGRAMME_MODE_ID = CPG.PROGRAMME_MODE
                                      INNER JOIN STF_PERSONAL_INFO AS SP ON SP.DEPARTMENT=CP.DEPARTMENT
                                          INNER JOIN
                                      SUP_SHIFT AS SS ON SS.SHIFT_ID = CPG.SHIFT
                                 AND CPG.IS_DELETED != 1 AND SP.STAFF_ID=?STAFF_ID;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchConvocationPaymentReport:
                    {
                        query = @"  SELECT 
                                            SP.STUDENT_ID,
                                            SP.FIRST_NAME,
                                            DATE_FORMAT(SP.DATE_OF_BIRTH, '%d-%m-%Y') AS DATE_OF_BIRTH,
                                            PROGRAMME_DESCRIPTION AS PROGRAMME_NAME,
                                            REGISTER_NO,
                                            ROLL_NO,
                                            SB.BATCH,
                                            CE.APPROVAL,
                                            CASE
                                                WHEN SUM(IFNULL(O.CREDIT, 0)) = 0 THEN 'NOT PAID'
                                                WHEN SUM(IFNULL(O.CREDIT, 0)) > SUM(IFNULL(O.DEBIT, 0)) THEN 'NOT PAID'
                                                ELSE 'PAID'
                                            END AS STATUS,
                                            GROUP_CONCAT(DATE_FORMAT(ft.PAYMENT_DATE, '%d-%m-%Y')) AS PAYMENT_DATE,
                                            CS.SETTING_NAME,
                                            Sum(O.DEBIT) AS TOTAL_AMOUNT
                                        FROM
                                            STU_PERSONAL_INFO AS SP
                                                INNER JOIN
                                            CONVOCATION_ELEGIBILITY AS CE ON SP.STUDENT_ID = CE.STUDENT_ID
                                                INNER JOIN
                                            CONVOCATION_SETTING AS CS ON CS.SETTING_ID = CE.SETTING_ID
                                                INNER JOIN
                                            CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                                INNER JOIN
                                            CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                                INNER JOIN
                                            SUP_BATCHES AS SB ON SP.BATCH = SB.BATCH_ID
                                                LEFT JOIN
                                            FEE_FREQUENCY_FEE_MODE AS FFM ON ffm.FEE_MODE = ?FEE_MODE
                                                LEFT JOIN
                                            FEE_STUDENT_ACCOUNT AS O ON O.STUDENT_ID = SP.STUDENT_ID and O.FREQUENCY_ID = FFM.FREQUENCY_ID
                                                LEFT JOIN
                                            SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFM.FREQUENCY_ID
                                                LEFT JOIN
                                            FEE_TRANSACTION AS FT ON FT.TRANSACTION_ID = O.TRANSACTION_ID
                                                AND O.IS_DELETED != 1
                                                AND ft.IS_DELETED != 1
                                               
                                                AND ('?FROM_DATE' = ''
                                                OR DATE(FT.PAYMENT_DATE) >= '?FROM_DATE')
                                                AND ('?TO_DATE' = ''
                                                OR DATE(FT.PAYMENT_DATE) <= '?TO_DATE')
                                        WHERE
                                            CE.SETTING_ID = ?SETTING_ID AND SP.IS_DELETED != 1
                                                AND CE.IS_DELETED != 1
                                                AND CPG.PROGRAMME_GROUP_ID in (?PROGRAMME_ID)
                                        GROUP BY SP.STUDENT_ID
                                        ORDER BY PROGRAMME_NAME , ROLL_NO;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchConvocationPaidOnlyReport:
                    {
                        query = @"  SELECT 
                                            SP.STUDENT_ID,
                                            SP.FIRST_NAME,
                                            DATE_FORMAT(SP.DATE_OF_BIRTH, '%d-%m-%Y') AS DATE_OF_BIRTH,
                                            PROGRAMME_DESCRIPTION AS PROGRAMME_NAME,
                                            REGISTER_NO,
                                            ROLL_NO,
                                            SB.BATCH,
                                            CE.APPROVAL,
                                            'PAID' AS STATUS,
                                            GROUP_CONCAT(DATE_FORMAT(ft.PAYMENT_DATE, '%d-%m-%Y')) AS PAYMENT_DATE,
                                            CS.SETTING_NAME,
                                            SUM(O.DEBIT) AS TOTAL_AMOUNT
                                        FROM
                                            STU_PERSONAL_INFO AS SP
                                                INNER JOIN
                                            CONVOCATION_ELEGIBILITY AS CE ON SP.STUDENT_ID = CE.STUDENT_ID
                                                INNER JOIN
                                            CONVOCATION_SETTING AS CS ON CS.SETTING_ID = CE.SETTING_ID
                                                INNER JOIN
                                            CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                                INNER JOIN
                                            CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                                INNER JOIN
                                            SUP_BATCHES AS SB ON SP.BATCH = SB.BATCH_ID
                                                INNER JOIN
                                            FEE_FREQUENCY_FEE_MODE AS FFM ON ffm.FEE_MODE = ?FEE_MODE
                                                INNER JOIN
                                            FEE_STUDENT_ACCOUNT AS O ON O.STUDENT_ID = SP.STUDENT_ID and O.FREQUENCY_ID = FFM.FREQUENCY_ID
                                                LEFT JOIN
                                            SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFM.FREQUENCY_ID
                                                LEFT JOIN
                                            FEE_TRANSACTION AS FT ON FT.TRANSACTION_ID = O.TRANSACTION_ID
                                        WHERE
                                            CE.SETTING_ID = ?SETTING_ID AND SP.IS_DELETED != 1
                                                AND CE.IS_DELETED != 1
                                                AND O.IS_DELETED != 1
                                                AND ft.IS_DELETED != 1
                                                AND CPG.PROGRAMME_GROUP_ID in (?PROGRAMME_ID)
                                                AND ('?FROM_DATE' = '' OR DATE(FT.PAYMENT_DATE) >= '?FROM_DATE')
                                                AND ('?TO_DATE' = '' OR DATE(FT.PAYMENT_DATE) <= '?TO_DATE')
                                        GROUP BY SP.STUDENT_ID
                                        ORDER BY PROGRAMME_NAME , ROLL_NO;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchConvocationRegisteredReport:
                    {
                        query = @"  SELECT 
                                            SP.STUDENT_ID,
                                            SP.FIRST_NAME,
                                            DATE_FORMAT(SP.DATE_OF_BIRTH, '%d-%m-%Y') AS DATE_OF_BIRTH,
                                            PROGRAMME_DESCRIPTION AS PROGRAMME_NAME,
                                            REGISTER_NO,
                                            ROLL_NO,
                                            SB.BATCH,
                                            CE.APPROVAL,
                                            CASE
                                                WHEN SUM(IFNULL(O.CREDIT, 0)) = 0 THEN 'NOT PAID'
                                                WHEN SUM(IFNULL(O.CREDIT, 0)) > SUM(IFNULL(O.DEBIT, 0)) THEN 'NOT PAID'
                                                ELSE 'PAID'
                                            END AS STATUS,
                                            GROUP_CONCAT(DATE_FORMAT(ft.PAYMENT_DATE, '%d-%m-%Y')) AS PAYMENT_DATE,
                                            CS.SETTING_NAME,
                                            SUM(O.DEBIT) AS TOTAL_AMOUNT
                                        FROM
                                            STU_PERSONAL_INFO AS SP
                                                INNER JOIN
                                            CONVOCATION_ELEGIBILITY AS CE ON SP.STUDENT_ID = CE.STUDENT_ID
                                                INNER JOIN
                                            CONVOCATION_SETTING AS CS ON CS.SETTING_ID = CE.SETTING_ID
                                                INNER JOIN
                                            CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                                INNER JOIN
                                            CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                                INNER JOIN
                                            SUP_BATCHES AS SB ON SP.BATCH = SB.BATCH_ID
                                                INNER JOIN
                                            FEE_FREQUENCY_FEE_MODE AS FFM ON ffm.FEE_MODE = ?FEE_MODE
                                                INNER JOIN
                                            FEE_STUDENT_ACCOUNT AS O ON O.STUDENT_ID = SP.STUDENT_ID AND O.FREQUENCY_ID = FFM.FREQUENCY_ID
                                                LEFT JOIN
                                            SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFM.FREQUENCY_ID
                                                LEFT JOIN
                                            FEE_TRANSACTION AS FT ON FT.TRANSACTION_ID = O.TRANSACTION_ID
                                                AND O.IS_DELETED != 1
                                                AND (ft.IS_DELETED IS NULL OR ft.IS_DELETED != 1)
                                        WHERE
                                            CE.SETTING_ID = ?SETTING_ID AND SP.IS_DELETED != 1
                                                AND CE.IS_DELETED != 1
                                                AND CPG.PROGRAMME_GROUP_ID in (?PROGRAMME_ID)
                                        GROUP BY SP.STUDENT_ID
                                        ORDER BY PROGRAMME_NAME , ROLL_NO;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchSupAluPresentStatus:
                    {
                        query = @"SELECT PRESENT_STATUS_ID, PRESENT_STATUS_NAME FROM sup_alu_present_status WHERE IS_DELETED != 1;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchConvocationEligbleStudnet:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        SP.FIRST_NAME,
                                        DATE_FORMAT(SP.DATE_OF_BIRTH, '%d-%m-%Y') AS DATE_OF_BIRTH,
                                        PROGRAMME_DESCRIPTION AS PROGRAMME_NAME,
                                        REGISTER_NO,
                                        cc.CLASS_NAME,
                                        CC.CLASS_ID,
                                        ROLL_NO,
                                        SB.BATCH,
                                        CE.APPROVAL,
                                        CASE
                                            WHEN SUM(IFNULL(O.CREDIT, 0)) = 0 THEN 'NOT PAID'
                                            WHEN SUM(IFNULL(O.CREDIT, 0)) > SUM(IFNULL(O.DEBIT, 0)) THEN 'NOT PAID'
                                            ELSE 'PAID'
                                        END AS STATUS,
                                        GROUP_CONCAT(DATE_FORMAT(ft.PAYMENT_DATE, '%d-%m-%Y')) AS PAYMENT_DATE,
                                        CS.SETTING_NAME,
                                        SUM(DISTINCT O.DEBIT) AS TOTAL_AMOUNT
                                    FROM
                                        STU_PERSONAL_INFO AS SP
                                            INNER JOIN
                                        CONVOCATION_ELEGIBILITY AS CE ON SP.STUDENT_ID = CE.STUDENT_ID
                                            INNER JOIN
                                        CONVOCATION_SETTING AS CS ON CS.SETTING_ID = CE.SETTING_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                            INNER JOIN
                                        (SELECT 
                                            STUDENT_ID, MAX(ACADEMIC_YEAR) AS MAX_YEAR
                                        FROM
                                            STU_CLASS
                                        GROUP BY STUDENT_ID) AS SC_MAX ON SC_MAX.STUDENT_ID = SP.STUDENT_ID
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SC_MAX.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = SC_MAX.MAX_YEAR
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        SUP_BATCHES AS SB ON SP.BATCH = SB.BATCH_ID
                                            LEFT JOIN
                                        FEE_FREQUENCY_FEE_MODE AS FFM ON ffm.FEE_MODE = ?FEE_MODE
                                            LEFT JOIN
                                        FEE_STUDENT_ACCOUNT AS O ON O.STUDENT_ID = SP.STUDENT_ID
                                            AND O.FREQUENCY_ID = FFM.FREQUENCY_ID
                                            LEFT JOIN
                                        SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFM.FREQUENCY_ID
                                            LEFT JOIN
                                        FEE_TRANSACTION AS FT ON FT.TRANSACTION_ID = O.TRANSACTION_ID
                                            AND O.IS_DELETED != 1
                                            AND ft.IS_DELETED != 1
                                            AND ('?FROM_DATE' = ''
                                            OR DATE(FT.PAYMENT_DATE) >= '?FROM_DATE')
                                            AND ('?TO_DATE' = ''
                                            OR DATE(FT.PAYMENT_DATE) <= '?TO_DATE')
                                    WHERE
                                        CE.SETTING_ID = ?SETTING_ID AND SP.IS_DELETED != 1
                                            AND CE.IS_DELETED != 1
                                           AND CPG.PROGRAMME_GROUP_ID in (?PROGRAMME_ID)
                                    GROUP BY SP.STUDENT_ID
        ORDER BY PROGRAMME_NAME , ROLL_NO;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchAlumniReportData:
                    {
                        query = @"SELECT 
                                    A.SALUTATION,
                                    A.NAME,
                                    A.ROLL_NO,
                                    IFNULL(CP.PROGRAMME_NAME, A.PROGRAMME) AS PROGRAMME,
                                    A.BRANCH_UG,
                                    A.BRANCH_PG,
                                    A.BRANCH_MPHIL,
                                    A.BRANCH_PHD,
                                    A.BATCH,
                                    concat('SHIFT -',SPS.SHIFT_NAME)  as SHIFT_NAME,
                                    A.PRESENT_STATUS,
                                    A.DESIGNATION,
                                    A.COMPANY,
                                    A.PERMANENT_ADDRESS,
                                    A.COMMUNICATION_ADDRESS,
                                    A.MOBILE_NO,
                                    A.EMAIL_ID,
                                    A.OTHER_DETAILS,
                                    IFNULL(DATE_FORMAT(A.DATE_OF_BIRTH, '%d-%m-%Y'), A.DATE_OF_BIRTH) AS DATE_OF_BIRTH,
                                    IFNULL(DATE_FORMAT(A.CREATED_DATE, '%d-%m-%Y'), A.CREATED_DATE) AS CREATED_DATE,
                                    A.PHOTO
                                FROM ALUMNI_DATA_COLLECTION A
                                LEFT JOIN CP_PROGRAMME_GROUP CPG ON CPG.PROGRAMME_GROUP_ID = A.PROGRAMME
                                LEFT JOIN CP_PROGRAMME CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                INNER JOIN SUP_SHIFT SPS ON SPS.SHIFT_ID = A.STREAM
                                WHERE A.STREAM IN (?SHIFT_ID) 
                                    AND A.PROGRAMME IN (?PROGRAMME_ID)
                                    AND ('?FROM_DATE' = '' OR COALESCE(STR_TO_DATE(REPLACE(A.CREATED_DATE, '/', '-'), '%d-%m-%Y'), DATE(A.CREATED_DATE)) >= '?FROM_DATE')
                                    AND ('?TO_DATE' = '' OR COALESCE(STR_TO_DATE(REPLACE(A.CREATED_DATE, '/', '-'), '%d-%m-%Y'), DATE(A.CREATED_DATE)) <= '?TO_DATE')
                                ORDER BY A.CREATED_DATE DESC;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchPendingAlumniDataCollection:
                    {
                        query = @"SELECT 
                                    SP.ROLL_NO,
                                    UPPER(TRIM(SP.FIRST_NAME)) AS NAME,
                                    SB.BATCH AS BATCH,
                                    CP.PROGRAMME_DESCRIPTION AS PROGRAMME,
                                    ACF.EMAIL AS EMAIL_ID,
                                    ACF.PHONE_NO AS MOBILE_NO
                                FROM
                                    ACF_STU_INFO AS ACF
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = ACF.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                        INNER JOIN
                                    CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = CP.DEPARTMENT
                                        INNER JOIN
                                    CP_CLASSES AS CPC ON CPC.CLASS_ID = SC.CLASS_ID
                                        LEFT JOIN
                                    SUP_BATCHES AS SB ON SB.BATCH_ID = SC.BATCH AND SB.IS_DELETED != 1
                                WHERE
                                    ACF.IS_DELETED != 1
                                        AND SP.IS_DELETED != 1
                                        AND SC.IS_DELETED != 1
                                        AND SC.ACADEMIC_YEAR = (
                                            SELECT MAX(ACADEMIC_YEAR)
                                            FROM STU_CLASS
                                            WHERE STUDENT_ID = SP.STUDENT_ID
                                                AND IS_DELETED != 1
                                        )
                                        AND SP.REGISTER_NO NOT IN (
                                            SELECT DISTINCT IFNULL(ROLL_NO, '') FROM alumni_data_collection
                                        )
                                        AND (COALESCE(SP.ROLL_NO, '') = '' OR SP.ROLL_NO NOT IN (
                                            SELECT DISTINCT IFNULL(ROLL_NO, '') FROM alumni_data_collection
                                        ))
                                 ORDER BY SC.BATCH DESC, PROGRAMME, ROLL_NO;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchStudentInfoForAlumniImportByRollNo:
                    {
                        query = @"SELECT 
                                    SP.ROLL_NO,
                                    CASE 
                                        WHEN SP.GENDER IN ('Male', 'M', 'MALE') THEN 'Mr.' 
                                        WHEN SP.GENDER IN ('Female', 'F', 'FEMALE') THEN 'Ms.' 
                                        ELSE '' 
                                    END AS SALUTATION,
                                    UPPER(TRIM(SP.FIRST_NAME)) AS NAME,
                                    SC.PROGRAMME_GROUP_ID AS PROGRAMME,
                                    SB.BATCH AS BATCH,
                                    SP.SHIFT_ID AS STREAM,
                                    ACF.EMAIL AS EMAIL_ID,
                                    ACF.PHONE_NO AS MOBILE_NO,
                                    ACF.AADHAR_NO AS AADHAR_NO,
                                    COALESCE(NULLIF(TRIM(AP.SUGGESTIONS), ''), 'ACF Import') AS OTHER_DETAILS,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH, '%Y-%m-%d') AS DATE_OF_BIRTH,
                                    SP.PHOTO,
                                    SC.ACADEMIC_YEAR,
                                    COALESCE(NULLIF(TRIM(AP.COURSE_NAME), ''), AP.DESIGNATION, '') AS DESIGNATION,
                                    COALESCE(AP.INSTITUTION_COMPANY_NAME, '') AS COMPANY,
                                    COALESCE(
                                        NULLIF(TRIM(AP.INSTITUTION_COMPANY_ADDRESS), ''),
                                        CONCAT(
                                            IFNULL(AD.P_DOOR_DETAIL, ''), ' ', 
                                            IFNULL(AD.P_STREET, ''), ' ', 
                                            IFNULL(AD.P_VILLAGE_AREA, ''), ' ', 
                                            IFNULL(AD.P_CITY, '')
                                        )
                                    ) AS PERMANENT_ADDRESS,
                                    COALESCE(
                                        NULLIF(TRIM(AP.INSTITUTION_COMPANY_ADDRESS), ''),
                                        CONCAT(
                                            IFNULL(AD.C_DOOR_DETAIL, ''), ' ', 
                                            IFNULL(AD.C_STREET, ''), ' ', 
                                            IFNULL(AD.C_VILLAGE_AREA, ''), ' ', 
                                            IFNULL(AD.C_CITY, '')
                                        )
                                    ) AS COMMUNICATION_ADDRESS
                                FROM
                                    ACF_STU_INFO AS ACF
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = ACF.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID
                                        LEFT JOIN
                                    STU_ADDRESS AS AD ON AD.STUDENT_ID = SP.STUDENT_ID AND AD.IS_DELETED != 1
                                        LEFT JOIN
                                    ALUMNAE_PROGRESSION AS AP ON AP.STUDENT_ID = SP.STUDENT_ID AND AP.IS_DELETED != 1
                                        LEFT JOIN
                                    SUP_BATCHES AS SB ON SB.BATCH_ID = SC.BATCH AND SB.IS_DELETED != 1
                                WHERE
                                    ACF.IS_DELETED != 1
                                        AND SP.IS_DELETED != 1
                                        AND SC.IS_DELETED != 1
                                        AND SC.ACADEMIC_YEAR = (
                                            SELECT MAX(ACADEMIC_YEAR)
                                            FROM STU_CLASS
                                            WHERE STUDENT_ID = SP.STUDENT_ID
                                                AND IS_DELETED != 1
                                        )
                                        AND SP.ROLL_NO = ?ROLL_NO;";
                        break;
                    }
                case AlumnaeSQLCommands.FetchProgrammeWiseAlumniReport:
                    {
                        query = @"SELECT 
                                    CP.PROGRAMME_NAME AS PROGRAMME_NAME,
                                    SAT.APPLICATION_TYPE AS APPLICATION_TYPE,
                                    COUNT(A.ROLL_NO) AS COUNT
                                FROM
                                    alumni_data_collection AS A
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = A.PROGRAMME
                                        INNER JOIN
                                    CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                        INNER JOIN
                                    SUP_APPLICATION_TYPE AS SAT ON SAT.APPLICATION_TYPE_ID = CPG.APPTYPE_ID
                                GROUP BY 
                                    CP.PROGRAMME_NAME, SAT.APPLICATION_TYPE
                                ORDER BY 
                                    APPLICATION_TYPE, PROGRAMME_NAME;";
                        break;
                    }
            }
            return query;
        }
    }
}
