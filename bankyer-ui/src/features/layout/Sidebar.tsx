import { AppShell, Divider, NavLink, Stack, Text, Title } from '@mantine/core'

export function Sidebar() {
  return (
    <AppShell.Navbar p="md">
      <Stack gap="lg" h="100%">
        <div>
          <Text c="dimmed" fw={700} size="xs" tt="uppercase">
            Workspace
          </Text>
          <NavLink active label="Accounts" mt="xs" />
          <NavLink disabled label="Transactions" />
          <NavLink disabled label="Transfers" />
        </div>

        <Divider />

        <div>
          <Text c="dimmed" fw={700} size="xs" tt="uppercase">
            Administration
          </Text>
          <NavLink disabled label="Users" mt="xs" />
          <NavLink disabled label="Settings" />
        </div>

        <div style={{ marginTop: 'auto' }}>
          <Title order={4}>Bankyer</Title>
          <Text c="dimmed" size="sm">
            Banking operations
          </Text>
        </div>
      </Stack>
    </AppShell.Navbar>
  )
}
