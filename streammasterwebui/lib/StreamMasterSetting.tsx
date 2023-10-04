import React from 'react'
import { getIconUrl } from './common/common'
import { AuthenticationType } from './common/streammaster_enums'
import { SettingDto, useSettingsGetSettingQuery } from './iptvApi'

const StreamMasterSetting = (): StreamMasterSettingResponse => {
  const settingsQuery = useSettingsGetSettingQuery()
  const [isLoading, setIsLoading] = React.useState<boolean>(true)
  const [isDebug, setIsDebug] = React.useState<boolean>(true)
  const [data, setData] = React.useState<SettingDto>({} as SettingDto)
  const [streamMasterIcon, setStreamMasterIcon] = React.useState<string>('')

  const [cacheIcon, setCacheIcon] = React.useState<boolean>(false)
  const [defaultIcon, setDefaultIcon] = React.useState<string>('')
  const [defaultIconUrl, setDefaultIconUrl] = React.useState<string>('')
  const [defaultIconName, setDefaultIconName] = React.useState<string>('')
  const [authenticationType, setAuthenticationType] =
    React.useState<AuthenticationType>(AuthenticationType.None)

  React.useEffect(() => {
    if (
      settingsQuery.isLoading ||
      !settingsQuery.data ||
      settingsQuery.data === undefined
    ) {
      return
    }

    setIsLoading(true)

    if (settingsQuery.data.authenticationMethod)
      setAuthenticationType(settingsQuery.data.authenticationMethod)

    if (settingsQuery.data.defaultIcon) {
      // const base = settingsQuery.data.urlBase !== '' ? settingsQuery.data.urlBase : '/';
      setStreamMasterIcon('images/StreamMaster.png')
      setDefaultIcon(settingsQuery.data.defaultIcon)
      setDefaultIconName(settingsQuery.data.defaultIcon)
      setCacheIcon(settingsQuery.data.cacheIcons === true)
      setIsDebug(settingsQuery.data.isDebug === true)
      const url = getIconUrl(
        settingsQuery.data.defaultIcon,
        settingsQuery.data.defaultIcon,
        settingsQuery.data.cacheIcons === true,
      )

      setDefaultIconUrl(url)
    }

    setData(settingsQuery.data)
    setIsLoading(false)
  }, [settingsQuery.data, settingsQuery.isLoading])

  return {
    authenticationType,
    cacheIcon,
    data,
    defaultIcon,
    defaultIconName,
    defaultIconUrl,
    isDebug,
    isLoading,
    streamMasterIcon,
  }
}

export type StreamMasterSettingResponse = {
  authenticationType: AuthenticationType
  cacheIcon: boolean
  data: SettingDto
  defaultIcon: string
  defaultIconName: string
  defaultIconUrl: string
  isDebug: boolean
  isLoading: boolean
  streamMasterIcon: string
}

export default StreamMasterSetting
