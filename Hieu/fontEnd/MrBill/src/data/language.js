var LANGUAGE_CULTURE = [
    {
        code: 'sv',
        displayName: 'Swedish',
        value: 'sv',
        label: 'Swedish',
    },
    {
        code: 'en',
        displayName: 'English',
        value: 'en',
        label: 'English',
    }
];

var MAIL_SETTING = {
        sv :[
        {
            code: 'sv',
            displayName: 'Send me e-mails with 10MB files',
            value: 'file',
            label: 'Skicka e-mail med 10MB filer',
        },
        {
            code: 'en',
            displayName: 'Send me a link to the PDF',
            value: 'link',
            label: 'Skicka enbart e-mail med länk där filen kan laddas ner',
        }],
        en:[
         {
            code: 'sv',
            displayName: 'Send me e-mails with 10MB files',
            value: 'file',
            label: 'Send me e-mails with 10MB files',
        },
        {
            code: 'en',
            displayName: 'Send me a link to the PDF',
            value: 'link',
            label: 'Send me a link to the PDF',
        }
        ]
    };

var MONTHS = [
    {value: 1, sv: 'Januari', en: 'January'},
    {value: 2, sv: 'Februari', en: 'February'},
    {value: 3, sv: 'Mars', en: 'March'},
    {value: 4, sv: 'April', en: 'April'},
    {value: 5, sv: 'Maj', en: 'May'},
    {value: 6, sv: 'Juni', en: 'June'},
    {value: 7, sv: 'Juli', en: 'July'},
    {value: 8, sv: 'Augusti', en: 'August'},
    {value: 9, sv: 'September', en: 'September'},
    {value: 10, sv: 'Oktober', en: 'October'},
    {value: 11, sv: 'November', en: 'November'},
    {value: 12, sv: 'December', en: 'December'},
];

var APPLICATION_LANGUAGES = {
    // COMMON
    loadingPopup:{
        popupLoading: {
            sv: 'Loading',
            en: 'Loading'
        },
        popupDone: {
            sv: 'Gjort!',
            en: 'Done!'
        },
    },
    common: {
        popupLoading: {
            sv: 'Loading',
            en: 'Loading'
        },
        skipButton: {
            sv: 'HOPPA ÖVER',
            en: 'SKIP',
        },
        nextButton: {
            sv: 'NÄSTA',
            en: 'NEXT',
        },
    },
    // MENU
    menu: {
        camera: {
            sv: 'Bifoga Kvitto',
            en: 'Attach Receipt'
        },
        transaction: {
            sv: 'Transaktioner',
            en: 'Transactions',
        },
        logout: {
            sv: 'Logga ut',
            en: 'Log out',
        },
        guide: {
            sv: 'Guide',
            en: 'Guide',
        },
        setting: {
            sv: 'Setting',
            en: 'Setting',
        },
    },
    // LOGIN PAGE
    loginPage: {
        warningLoginBlank: {
            sv: 'Please enter email and password!',
            en: 'Please enter email and password!',
        },
        warningLoginFail: {
            sv: 'Login failed. Please check email and password!',
            en: 'Login failed. Please check email and password!',
        },
        emailTextbox: {
            sv: 'Email',
            en: 'Email',
        },
        passwordTextbox: {
            sv: 'Lösenord',
            en: 'Password',
        },
        loginButton: {
            sv: 'LOGGA IN',
            en: 'SIGN IN',
        },
        createAccountButton: {
            sv: 'SKAPA KONTO',
            en: 'CREATE ACCOUNT',
        },
        forgotPasswordButton: {
            sv: 'Glömt lösenord?',
            en: 'Forgot password?',
        },
        laguage: {
            sv: 'Språk:',
            en: 'Language:'
        },        
    }, 
    // CREATE USER

    createUser: {
        warningLoginBlank: {
            sv: 'Please enter account infomation!',
            en: 'Please enter account infomation!',
        },
        warningLoginFail: {
            sv: 'Login failed. Please check email and password!',
            en: 'Login failed. Please check email and password!',
        },
        emailTextbox: {
            sv: 'Email',
            en: 'Email',
        },
        passwordTextbox: {
            sv: 'Lösenord',
            en: 'Password',
        },
        loginButton: {
            sv: 'LOGGA IN',
            en: 'SIGN IN',
        },
        createAccountButton: {
            sv: 'SKAPA KONTO',
            en: 'CREATE ACCOUNT',
        },
        forgotPasswordButton: {
            sv: 'Glömt lösenord?',
            en: 'Forgot password?',
        },
        laguage: {
            sv: 'Språk:',
            en: 'Language:'
        },
        back: {
            sv: 'TILLBAKA',
            en: 'BACK',
        },
        header:{
            sv: 'Skapa konto',
            en: 'Create Account',
        },   
        register:{
            sv: 'Register',
            en: 'Register',
        }, 
        firstName:{
            sv: 'Förnamn',
            en: 'First name',
        },
        lastName:{
            sv:'Efternamn',
            en: 'Last name',
        },
         password:{
            sv:'Lösenord',
            en: 'Password',
        },
         confirmPassword:{
            sv:'Bekfräfta Lösenord',
            en: 'Re-type password',
        },
        wrongEmail:{
            sv:'The Email field is not a valid e-mail address.',
            en: 'The Email field is not a valid e-mail address.',
        },
        passNotMath:{
            sv:'Lösenorden matchar inte',
            en:'Lösenorden matchar inte',   
        }   

    }, 

    // SETTING

    setting: {
         back:{
            sv: 'TILLBAKA',
            en: 'BACK',
        },
        header:{
            sv: 'User Setting',
            en: 'User Setting',
        },  
        mail_setting:{
            sv: "E-mail (bifogade filer)",
            en: 'E-mail (attached files)',
        },         
         save: {
            sv: 'SPARA',
            en: 'SAVE',
        },
        updateSetting:{
            sv:'Uppdatera inställningar',
            en:'E-mail (attached files)'
        }
    }, 

    // GUID STEP 1
    guideStep1Page: {
        header: {
            sv: '1. Fota',
            en: '1. Fota',
        },
        text: {
            sv: 'Ta en bild på ditt kvitto!',
            en: 'Take a picture of your receipt!',
        },
    },
    // GUID STEP 2
    guideStep2Page: {
        header: {
            sv: '2. Komplettera',
            en: '2. Complete',
        },
        text: {
            sv: 'Fyll i detaljer för ditt utlägg.',
            en: 'Fill in the details for your outlay.',
        },
    },    
    // GUID STEP 3
    guideStep3Page: {
        header: {
            sv: '3. Synkronisera',
            en: '3. Synchronize',
        },
        text: {
            sv: 'Synkronisera ditt utlägg med resten av dina utlägg!',
            en: 'Synchronize your expenses with the rest of your expenses!',
        },
    },
    // CAMERA
    cameraPage: {
        textReceipt: {
            sv: 'Fota ett kvitto eller',
            en: 'Photograph a receipt or',
        },
        textPhoto: {
            sv: 'bifoga ett foto',
            en: 'attach a photo',
        },
        popupGallery: {
            sv: 'Välj bild från telefonen',
            en: 'Attach a photo',
        },
        popupCamera: {
            sv: 'Fota kvitto',
            en: 'Photograph a receipt',
        },
        popupCancel: {
            sv: 'Cancel',
            en: 'Cancel',
        },
        warningCameraNotSupport: {
            sv: 'Camera API not supported',
            en: 'Camera API not supported',
        },
        warningPhotoEmpty: {
            sv: 'Please take a photo.',
            en: 'Please take a photo.',
        },
    },
    // TRANSACTION LIST
    transactionList: {
        header: {
            sv: 'Transaktioner',
            en: 'Transactions',
        },
        back: {
            sv: 'TILLBAKA',
            en: 'BACK',
        },
        synchronize: {
            sv: 'SYNKRONISERA KVITTON',
            en: 'SYNCHRONIZE RECEIPTS',
        },
        noTransaction: {
            sv: 'Ingen Transaktion!',
            en: 'No transaction!',
        },
        reloadRelease:{
            sv: 'Släpp för att uppdatera...',
            en: 'Release to refresh...',
        },
        reloadPulldown:{
            sv: 'Rullgardins att uppdatera...',
            en: 'Pulldown to refresh...',
        },
        reloadLoading:{
            sv: 'Läser in...',
            en: 'Loading...',
        },
        syncTransaction:{
            sv: 'Vill du skicka denna månads sammanställning till din e-mail?',
            en: "Do you want to send this month's report to your e-mail?",
        },
        notSyncTransaction:{
            sv: 'Du måste synkronisera dina transaktioner innan du kan skicka din månadssammanställning. Vill du synkronisera nu?',
            en: 'You have to synchronise your transactions before you can send your monthly report. Do you want to synchronise now?',
        },
        yesButton:{
            sv: 'Ja',
            en: 'Yes',
        },
        noButton:{
            sv: 'Nej',
            en: 'No',
        },
    },
    // TRANSACTION
    transaction: {
        header: {
            sv: 'UTLÄGG',
            en: 'OUTLAY',
        },
        back: {
            sv: 'TILLBAKA',
            en: 'BACK',
        },
        date: {
            sv: 'DATUM',
            en: 'DATE',
        },
        Supplier: {
            sv: 'LEVERANTÖR',
            en: 'SUPPLIER',
        },
        place: {
            sv: 'PLATS',
            en: 'PLACE',
        },
        Price: {
            sv: 'BELOPP',
            en: 'AMOUNT',
        },
        Vat: {
            sv: 'MOMS',
            en: 'VAT',
        },
        Currency: {
            sv: 'VALUTA',
            en: 'CURRENCY',
        },
        ExtraInfo: {
            sv: 'ÖVRIG KOMMENTAR',
            en: 'OTHER COMMENTS',
        },
        cancel: {
            sv: 'AVBRYT',
            en: 'CANCEL',
        },
        save: {
            sv: 'SPARA',
            en: 'SAVE',
        },
        confirmMessage:{
            sv: 'är du säker på att du vill spara förändringarna?',
            en: 'Are you sure you want to save all changes?'
        },
        next: {
            sv: 'NÄSTA',
            en: 'NEXT',
        },
        addMore: {
            sv: '+LÄGG TILL FLER',
            en: '+Add more',
        },
        warningEmptyField: {
            sv: 'Please enter value and picture.',
            en: 'Please enter value and picture.',
        },
    },
};


var Language = {
    getLanguageCulture: function() {
        return LANGUAGE_CULTURE;
    },

    getDownloadSetting: function() {
        return MAIL_SETTING;
    },


    getApplicationLanguages: function(page) {
        return APPLICATION_LANGUAGES[page];
    },

    getMonths: function(page) {
        return MONTHS;
    }, 
};

module.exports = Language;