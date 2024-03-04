# Application Setup Guide

This guide outlines the steps to create a new application in Azure Portal and set up necessary permissions and policies.

## Step 1: Create a New Application in Azure Portal

1. Go to Azure Portal at https://portal.azure.com.
2. Navigate to the App Registrations section.
3. Create a new application.

## Step 2: Configuration

1. Enter the name for the application.
2. Choose "Supported account types" as "Accounts in any organizational directory (Any Microsoft Entra ID tenant - Multitenant) and personal Microsoft accounts (e.g. Skype, Xbox)".

## Step 3: Add API Permissions

1. Click on "Add Permission".
2. Select "Microsoft Graph".
3. Choose the following scopes:
   - "user.read"
   - "mail.read"
   - "mail.send"
   - "OnlineMeetings.ReadWrite"
   - "Calendars.ReadWrite"

## Step 4: Generate Client Secret

1. Navigate to the "Certificates & Secrets" section.
2. Click on "New client secret".
3. Enter the name and expiration.
4. Copy the generated secret value.

## Step 5: Add Policies

1. Open PowerShell.
2. Execute the following commands:
   - Install-Module -Name MicrosoftTeams
   - Connect-MicrosoftTeams
   - New-CsApplicationAccessPolicy -Identity "policyName" -AppIds "yourClientID" -Description "yourDescription"
   - Grant-CsApplicationAccessPolicy -PolicyName "policyName" -Identity "userPrincipalName"
     OR
     - Grant the policy to a specific group:

       Grant-CsApplicationAccessPolicy -PolicyName "ASimplePolicy" -Identity "group@example.com" -GroupType Group

     - Grant the policy to the entire organization (tenant-wide):
       
       Grant-CsApplicationAccessPolicy -PolicyName "ASimplePolicy" -Global
     

Please replace placeholders such as "policyName", "yourClientID", "yourDescription", and "userPrincipalName" with your actual values.

