# Page snapshot

```yaml
- generic [ref=e4]:
  - heading "Sign In to CoinPay" [level=2] [ref=e5]
  - paragraph [ref=e6]:
    - text: "💡 For testing: Use"
    - strong [ref=e7]: testuser
    - text: or leave password empty
  - generic [ref=e8]:
    - generic [ref=e9]:
      - generic [ref=e10]: Username
      - textbox "Username" [ref=e11]:
        - /placeholder: Enter your username
    - generic [ref=e12]:
      - generic [ref=e13]: Password (optional for dev mode)
      - textbox "Password (optional for dev mode)" [ref=e14]:
        - /placeholder: Leave empty for dev login
    - button "Sign In" [ref=e15] [cursor=pointer]
  - paragraph [ref=e16]:
    - text: Don't have an account?
    - link "Sign up" [ref=e17] [cursor=pointer]:
      - /url: /register
```