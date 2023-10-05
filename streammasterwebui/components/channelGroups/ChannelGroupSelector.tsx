import { getChannelGroupMenuItem, getTopToolOptions } from '@/lib/common/common'
import { ResetLogoIcon } from '@/lib/common/icons'
import {
  useChannelGroupsGetChannelGroupIdNamesQuery,
  type ChannelGroupIdName,
} from '@/lib/iptvApi'
import { Button } from 'primereact/button'
import { Dropdown } from 'primereact/dropdown'
import React, { useCallback, useEffect, useState } from 'react'
import ChannelGroupAddDialog from './ChannelGroupAddDialog'

type ChannelGroupSelectorProps = {
  readonly className?: string
  readonly onChange: (value: string) => void
  readonly resetValue?: string
  readonly value?: string
}

const ChannelGroupSelector: React.FC<ChannelGroupSelectorProps> = ({
  className,
  onChange,
  resetValue,
  value,
}) => {
  const channelGroupNamesQuery = useChannelGroupsGetChannelGroupIdNamesQuery()
  const [channelGroup, setChannelGroup] = useState<
    ChannelGroupIdName | undefined
  >(undefined)

  const setChannelGroupByName = useCallback(
    (channelGroupName: string) => {
      if (channelGroupName && channelGroupNamesQuery.data) {
        const foundChannelGroup = channelGroupNamesQuery.data.find(
          (cg) => cg.name === channelGroupName,
        )
        if (foundChannelGroup) {
          setChannelGroup(foundChannelGroup)
        }
      }
    },
    [channelGroupNamesQuery.data],
  )

  // Update channel group when prop value changes
  useEffect(() => {
    if (value) {
      setChannelGroupByName(value)
    }
  }, [setChannelGroupByName, value])

  const handleResetClick = () => {
    if (resetValue && channelGroup?.name !== resetValue) {
      setChannelGroupByName(resetValue)
      onChange(resetValue)
    }
  }

  const footerTemplate = () => (
    <div className="p-1 align-items-center justify-content-center">
      <hr />
      <div className="flex gap-2 align-items-center justify-content-end">
        {value !== resetValue && resetValue && (
          <Button
            icon={<ResetLogoIcon sx={{ fontSize: 18 }} />}
            onClick={handleResetClick}
            rounded
            severity="warning"
            size="small"
            tooltip="Reset Group"
            tooltipOptions={getTopToolOptions}
          />
        )}
        <ChannelGroupAddDialog />
      </div>
    </div>
  )

  const selectedTemplate = useCallback((option: any) => {
    if (!option) return

    return <div className="">{option.name}</div>
  }, [])

  return (
    <div className="flex w-full">
      <Dropdown
        className={`w-full ${className}`}
        filter
        filterInputAutoFocus
        itemTemplate={(option) =>
          getChannelGroupMenuItem(
            option.name,
            option.name + '  |  ' + option.totalCount,
          )
        } // getChannelGroupMenuItem(option.id, option.name)}
        onChange={(e) => onChange(e.value.name)}
        optionLabel="name"
        options={channelGroupNamesQuery.data}
        panelFooterTemplate={footerTemplate}
        placeholder="No Group"
        value={channelGroup}
        valueTemplate={selectedTemplate}
      />
    </div>
  )
}

ChannelGroupSelector.displayName = 'Channel Group Dropdown'

export default React.memo(ChannelGroupSelector)
