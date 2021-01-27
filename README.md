# Delivery Approval Demo App

```text
Url: https://deliveryapprovaldemo.azurewebsites.net/webhook
```

### Logic Overview

1. Look in __ServiceConfig__ table in __deliveryapprovaldemost__ Azure Storage Account using the tenant from the request. If no config found for tenant, throw an error.

2. Look in __EmailOptions__ Seismic list for tenant and get OptStatus for each recipient email in request.

3. Get forbidden content from __ForbidContent__ Seismic list for each content item in request.

4. If recipient status = '_forbid_' then forbid all content for that recipient. If recipient status = '_allow_' or '_warn_', then use that status for each content item __UNLESS__ that content is forbidden.
