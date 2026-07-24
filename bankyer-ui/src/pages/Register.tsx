import { Alert, Anchor, Button, Paper, PasswordInput, Stack, Text, TextInput, Title } from '@mantine/core'
import { useForm } from '@mantine/form'
import { useMutation } from '@tanstack/react-query'
import { Link, useNavigate } from 'react-router'
import { authApi } from '../api/auth'

type RegisterValues = {
  email: string
  password: string
  confirmPassword: string
}

export function Register() {
  const navigate = useNavigate()
  const form = useForm<RegisterValues>({
    initialValues: { email: '', password: '', confirmPassword: '' },
    validate: {
      email: (value) => /^\S+@\S+$/.test(value) ? null : 'Enter a valid email address',
      password: (value) => value.length >= 6 ? null : 'Password must contain at least 6 characters',
      confirmPassword: (value, values) => value === values.password ? null : 'Passwords do not match',
    },
  })
  const registerMutation = useMutation({
    mutationFn: authApi.register,
    onSuccess: () => navigate('/login', { replace: true }),
  })

  return (
    <Paper withBorder p="xl" radius="md" w="100%" maw={420}>
      <Stack>
        <div>
          <Title order={2}>Create your account</Title>
          <Text c="dimmed" size="sm">Start managing your banking accounts securely.</Text>
        </div>

        <form onSubmit={form.onSubmit(({ email, password }) => registerMutation.mutate({ email, password }))}>
          <Stack>
            {registerMutation.error && (
              <Alert color="red" title="Could not create account">
                {registerMutation.error.message}
              </Alert>
            )}
            <TextInput label="Email" required type="email" {...form.getInputProps('email')} />
            <PasswordInput label="Password" required {...form.getInputProps('password')} />
            <PasswordInput label="Confirm password" required {...form.getInputProps('confirmPassword')} />
            <Button type="submit" loading={registerMutation.isPending}>Create account</Button>
          </Stack>
        </form>

        <Text c="dimmed" size="sm" ta="center">
          Already have an account? <Anchor component={Link} to="/login">Sign in</Anchor>
        </Text>
      </Stack>
    </Paper>
  )
}
