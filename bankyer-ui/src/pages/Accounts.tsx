import {
  Alert,
  Badge,
  Button,
  Container,
  Group,
  Modal,
  NumberInput,
  Paper,
  Select,
  SimpleGrid,
  Skeleton,
  Stack,
  Text,
  Title,
} from '@mantine/core'
import { useForm } from '@mantine/form'
import { useDisclosure } from '@mantine/hooks'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { accountsApi, type Account } from '../api/accounts'

const currencies = [
  { value: 'USD', label: 'USD — US Dollar' },
  { value: 'EUR', label: 'EUR — Euro' },
  { value: 'GBP', label: 'GBP — British Pound' },
  { value: 'JPY', label: 'JPY — Japanese Yen' },
  { value: 'AUD', label: 'AUD — Australian Dollar' },
  { value: 'CAD', label: 'CAD — Canadian Dollar' },
  { value: 'CHF', label: 'CHF — Swiss Franc' },
  { value: 'CNY', label: 'CNY — Chinese Yuan' },
  { value: 'SEK', label: 'SEK — Swedish Krona' },
  { value: 'NZD', label: 'NZD — New Zealand Dollar' },
]

function AccountCard({ account }: { account: Account }) {
  const balance = new Intl.NumberFormat(undefined, {
    style: 'currency',
    currency: account.currency.code,
  }).format(account.balance)

  return (
    <Paper withBorder p="lg" radius="md">
      <Stack gap="md">
        <Group justify="space-between" align="flex-start">
          <div>
            <Text c="dimmed" size="sm">Balance</Text>
            <Text fw={700} size="xl">{balance}</Text>
          </div>
          <Badge variant="light">{account.status}</Badge>
        </Group>

        <div>
          <Text fw={600}>{account.currency.code}</Text>
          <Text c="dimmed" size="sm">{account.currency.name}</Text>
        </div>

        <Text c="dimmed" size="xs" ff="monospace">{account.id}</Text>
      </Stack>
    </Paper>
  )
}

export function Accounts() {
  const [createOpened, { open: openCreate, close: closeCreate }] = useDisclosure(false)
  const queryClient = useQueryClient()
  const { data: accounts, error, isPending, refetch } = useQuery({
    queryKey: ['accounts'],
    queryFn: accountsApi.list,
  })
  const form = useForm({
    initialValues: { initialAmount: 100, currency: 'USD' },
    validate: {
      initialAmount: (value) => value > 0 ? null : 'Initial balance must be greater than zero',
      currency: (value) => value ? null : 'Select a currency',
    },
  })
  const createMutation = useMutation({
    mutationFn: accountsApi.create,
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ['accounts'] })
      form.reset()
      closeCreate()
    },
  })

  return (
    <Container size="lg" py="xl">
      <Group justify="space-between" mb="lg">
        <div>
          <Title order={1}>Accounts</Title>
          <Text c="dimmed">Overview of all accounts in the system</Text>
        </div>
        <Group>
          <Button onClick={openCreate}>Create account</Button>
          <Button variant="light" loading={isPending} onClick={() => refetch()}>
            Refresh
          </Button>
        </Group>
      </Group>

      <Modal opened={createOpened} onClose={closeCreate} title="Create account" centered>
        <form onSubmit={form.onSubmit((values) => createMutation.mutate(values))}>
          <Stack>
            {createMutation.error && (
              <Alert color="red" title="Could not create account">
                {createMutation.error.message}
              </Alert>
            )}
            <NumberInput
              label="Initial balance"
              description="A positive opening balance is required."
              decimalScale={2}
              min={0.01}
              required
              {...form.getInputProps('initialAmount')}
            />
            <Select
              label="Currency"
              data={currencies}
              required
              {...form.getInputProps('currency')}
            />
            <Group justify="flex-end" mt="sm">
              <Button variant="default" onClick={closeCreate}>Cancel</Button>
              <Button type="submit" loading={createMutation.isPending}>Create account</Button>
            </Group>
          </Stack>
        </form>
      </Modal>

      {error && (
        <Alert color="red" title="Could not load accounts" mb="lg">
          {error.message}
        </Alert>
      )}

      {isPending && (
        <SimpleGrid cols={{ base: 1, sm: 2, lg: 3 }}>
          {[0, 1, 2].map((index) => <Skeleton key={index} height={190} radius="md" />)}
        </SimpleGrid>
      )}

      {accounts?.length === 0 && (
        <Paper withBorder p="xl" radius="md">
          <Text ta="center" c="dimmed">There are no accounts yet.</Text>
        </Paper>
      )}

      {accounts && accounts.length > 0 && (
        <SimpleGrid cols={{ base: 1, sm: 2, lg: 3 }}>
          {accounts.map((account) => <AccountCard key={account.id} account={account} />)}
        </SimpleGrid>
      )}
    </Container>
  )
}
