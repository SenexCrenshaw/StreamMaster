import { camel2title } from '@/lib/common/common'
import Image from 'next/image'
import { type SyntheticEvent } from 'react'
import { LinkButton } from '../buttons/LinkButton'
import { type ColumnFieldType } from './DataSelectorTypes'
import getRecord from './getRecord'
import getRecordString from './getRecordString'

function epgSourceTemplate(tvgid: string) {
  return (
    <div>
      <div className="flex justify-content-start">{tvgid}</div>
    </div>
  )
}

function imageBodyTemplate(
  data: object,
  fieldName: string,
  defaultIcon: string,
) {
  const record = getRecordString(data, fieldName)

  return (
    <div className="flex flex-nowrap justify-content-center align-items-center p-0">
      <Image
        alt={record ?? 'Logo'}
        className="max-h-1rem max-w-full p-0"
        onError={(e: SyntheticEvent<HTMLImageElement, Event>) =>
          (e.currentTarget.src = e.currentTarget.src = +'/' + defaultIcon)
        }
        src={`${encodeURI(record ?? '')}`}
        style={{
          objectFit: 'contain',
        }}
      />
    </div>
  )
}

function streamsBodyTemplate(activeCount: string, totalCount: string) {
  if (activeCount === null || totalCount === undefined) {
    return null
  }

  return (
    <div className="flex align-items-center gap-2">
      {activeCount}/{totalCount}
    </div>
  )
}

function m3uLinkTemplate(data: object) {
  return <LinkButton link={getRecordString(data, 'm3ULink')} />
}

function epgLinkTemplate(data: object) {
  return <LinkButton link={getRecordString(data, 'xmlLink')} />
}

function urlTemplate(data: object) {
  return <LinkButton link={getRecordString(data, 'hdhrLink')} />
}

function blankTemplate() {
  return <div />
}

function isHiddenTemplate(data: object, fieldName: string) {
  const record = getRecord(data, fieldName)

  return record ? (
    <i className="pi pi-eye-slash text-blue-500" />
  ) : (
    <i className="pi pi-eye text-green-500" />
  )
}

function defaultTemplate(data: object, fieldName: string, camelize?: boolean) {
  let displayValue = JSON.stringify(getRecord(data, fieldName))

  if (displayValue.startsWith('"') && displayValue.endsWith('"')) {
    displayValue = displayValue.substring(1, displayValue.length - 1)
  }

  if (camelize) {
    displayValue = camel2title(displayValue)
  }

  return (
    <span
      style={{
        display: 'block',
        overflow: 'hidden',
        padding: '0rem !important',
        textOverflow: 'ellipsis',
        whiteSpace: 'nowrap',
      }}
    >
      {displayValue}
    </span>
  )
}

function bodyTemplate(
  data: object,
  fieldName: string,
  fieldType: ColumnFieldType,
  defaultIcon: string,
  camelize?: boolean,
) {
  switch (fieldType) {
    case 'blank':
      return blankTemplate()
    case 'm3ulink':
      return m3uLinkTemplate(data)
    case 'epglink':
      return epgLinkTemplate(data)
    case 'url':
      return urlTemplate(data)
    case 'epg':
      return epgSourceTemplate(getRecordString(data, 'user_Tvg_ID'))
    case 'image':
      return imageBodyTemplate(data, fieldName, defaultIcon)
    case 'streams':
      const activeCount = getRecord(data, 'activeCount')
      const totalCount = getRecord(data, 'totalCount')

      return streamsBodyTemplate(activeCount, totalCount)
    case 'isHidden':
      return isHiddenTemplate(data, fieldName)
    default:
      return defaultTemplate(data, fieldName, camelize)
  }
}

export default bodyTemplate
