import { AppShell, Burger, Center, Group, Loader, MantineProvider, Text, Title } from '@mantine/core'
import { useDisclosure } from '@mantine/hooks'
import { QueryClient, QueryClientProvider, useQuery } from '@tanstack/react-query'
import { Navigate, Route, Routes } from 'react-router'
import { authApi } from './api/auth'
import { Sidebar } from './features/layout/Sidebar'
import { Accounts } from './pages/Accounts'
import { Login } from './pages/Login'
import { Register } from './pages/Register'

const queryClient = new QueryClient()

export function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <MantineProvider defaultColorScheme="auto">
        <Routes>
          <Route path="/login" element={<AuthenticationPage><Login /></AuthenticationPage>} />
          <Route path="/register" element={<AuthenticationPage><Register /></AuthenticationPage>} />
          <Route path="*" element={<ProtectedApp />} />
        </Routes>
      </MantineProvider>
    </QueryClientProvider>
  )
}

function AuthenticationPage({ children }: { children: React.ReactNode }) {
  return <Center mih="100vh" p="md">{children}</Center>
}

function ProtectedApp() {
  const currentUserQuery = useQuery({
    queryKey: ['current-user'],
    queryFn: authApi.me,
    retry: false,
  })

  if (currentUserQuery.isPending) {
    return <Center mih="100vh"><Loader /></Center>
  }

  if (currentUserQuery.isError) {
    return <Navigate to="/login" replace />
  }

  return <ApplicationShell />
}

function ApplicationShell() {
  const [opened, { toggle }] = useDisclosure()

  return (
    <AppShell
      header={{ height: 60 }}
      navbar={{
        width: 264,
        breakpoint: 'sm',
        collapsed: { mobile: !opened },
      }}
      padding="md"
    >
      <AppShell.Header>
        <Group h="100%" px="md">
          <Burger
            hiddenFrom="sm"
            opened={opened}
            onClick={toggle}
            size="sm"
            aria-label="Toggle navigation"
          />
          <Title order={3}>Bankyer</Title>
          <Text c="dimmed" visibleFrom="sm">
            Banking operations
          </Text>
        </Group>
      </AppShell.Header>

      <Sidebar />

      <AppShell.Main>
        <Accounts />
      </AppShell.Main>
    </AppShell>
  )
}
