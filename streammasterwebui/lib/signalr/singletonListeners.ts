import { createSingletonListener } from './createSingletonListener'
import { hubConnection } from './signalr'

export const singletonChannelGroupsListener = createSingletonListener(
  'ChannelGroupsRefresh',
  hubConnection,
)
export const singletonEPGFilesListener = createSingletonListener(
  'EPGFilesRefresh',
  hubConnection,
)
export const singletonM3UFilesListener = createSingletonListener(
  'M3UFilesRefresh',
  hubConnection,
)
export const singletonProgrammesListener = createSingletonListener(
  'ProgrammesRefresh',
  hubConnection,
)
export const singletonSchedulesDirectListener = createSingletonListener(
  'SchedulesDirectsRefresh',
  hubConnection,
)
export const singletonSettingsListener = createSingletonListener(
  'SettingsRefresh',
  hubConnection,
)
export const singletonStreamGroupChannelGroupListener = createSingletonListener(
  'StreamGroupChannelGroupsRefresh',
  hubConnection,
)
export const singletonStreamGroupVideoStreamsListener = createSingletonListener(
  'StreamGroupVideoStreamsRefresh',
  hubConnection,
)
export const singletonStreamGroupsListener = createSingletonListener(
  'StreamGroupsRefresh',
  hubConnection,
)
export const singletonVideoStreamLinksListener = createSingletonListener(
  'VideoStreamLinksRefresh',
  hubConnection,
)
export const singletonVideoStreamLinksRemoveListener = createSingletonListener(
  'VideoStreamLinksRemove',
  hubConnection,
)
export const singletonVideoStreamsListener = createSingletonListener(
  'VideoStreamsRefresh',
  hubConnection,
)
export const singletonIconsListener = createSingletonListener(
  'IconsRefresh',
  hubConnection,
)
export const singletonLogsListener = createSingletonListener(
  'LogsRefresh',
  hubConnection,
)
export const singletonStatisticListener = createSingletonListener(
  'streamstatisticsresultsupdate',
  hubConnection,
)
