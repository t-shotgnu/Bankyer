import { Alert, Anchor, Button, Paper, PasswordInput, Stack, Text, TextInput, Title } from '@mantine/core'
import { useForm } from '@mantine/form'
import { useMutation } from '@tanstack/react-query'
import { Link, useNavigate } from 'react-router'
import { authApi } from '../api/auth'

export function Login() {
  const navigate = useNavigate()
  const form = useForm({
    initialValues: { email: '', password: '' },
    validate: {
      email: (value) => /^\S+@\S+$/.test(value) ? null : 'Enter a valid email address',
      password: (value) => value ? null : 'Enter your password',
    },
  })
  const loginMutation = useMutation({
    mutationFn: authApi.login,
    onSuccess: () => navigate('/', { replace: true }),
  })

  return (
    <Paper withBorder p="xl" radius="md" w="100%" maw={420}>
      <Stack>
        <div>
          <Title order={2}>Welcome back</Title>
          <Text c="dimmed" size="sm">Sign in to manage your accounts.</Text>
        </div>

        <form onSubmit={form.onSubmit((values) => loginMutation.mutate(values))}>
          <Stack>
            {loginMutation.error && (
              <Alert color="red" title="Could not sign in">
                {loginMutation.error.message}
              </Alert>
            )}
            <TextInput label="Email" required type="email" {...form.getInputProps('email')} />
            <PasswordInput label="Password" required {...form.getInputProps('password')} />
            <Button type="submit" loading={loginMutation.isPending}>Sign in</Button>
          </Stack>
        </form>

        <Text c="dimmed" size="sm" ta="center">
          New to Bankyer? <Anchor component={Link} to="/register">Create an account</Anchor>
        </Text>
      </Stack>
    </Paper>
  )
}
