import React, {useContext} from 'react'
import {Box, Button, Card,CardContent, CircularProgress, TextField, Typography} from '@mui/material'
import RegistrationStore from "./RegistrationStore";
import {contextStore} from "../../App";
import {observer} from "mobx-react-lite";

const Registration = () => {
    const appStore = useContext(contextStore);
    const store = new RegistrationStore(appStore.authStore);

    return (
        <Card
            sx={{
                marginTop:10,
                marginLeft: 55,
                marginRight: 55,
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
            }}
        >
        <CardContent >
            <Typography component="h1" variant="h5">
                Sign in
            </Typography>
            <Box component="form"
                 onSubmit={async (event) =>
                 {
                     event.preventDefault()
                     await store.registration()
                 }}
                 noValidate sx={{ mt: 1 }}>
                <TextField
                    margin="normal"
                    required
                    fullWidth
                    id="email"
                    label="Email Address"
                    name="email"
                    autoComplete="email"
                    onChange={(event) => store.changeEmail(event.target.value)}
                    autoFocus
                />
                <TextField
                    margin="normal"
                    required
                    fullWidth
                    name="password"
                    label="Password"
                    type="password"
                    id="password"
                    onChange={(event) => store.changePassword(event.target.value)}
                    autoComplete="current-password"
                />
                {!!store.error && (
                    <p style={{ color: 'red', fontSize: 14 }}>{store.error}</p>
                )}
                <Button
                    type="submit"
                    fullWidth
                    variant="contained"
                    sx={{ mt: 3, mb: 2 }}
                >
                    {store.isLoading ? (
                        <CircularProgress />
                    ) : (
                        'Submit'
                    )}
                </Button>
                {!!appStore.authStore.token && (
                    <p className="mt-3 mb-3" style={{ color: 'green', fontSize: 14, fontWeight: 700 }}>{`Success! Token is: ${appStore.authStore.token}`}</p>
                )}
            </Box>
        </CardContent>
        </Card>
    )
}

export default observer(Registration)