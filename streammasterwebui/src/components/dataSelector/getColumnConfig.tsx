import { camel2title } from '@/lib/common/common'
import { Tooltip } from 'primereact/tooltip'
import { type CSSProperties, type ReactNode } from 'react'
import { v4 as uuidv4 } from 'uuid'
import {
  type ColumnAlign,
  type ColumnFieldType,
  type ColumnMeta,
} from './DataSelectorTypes'

type ColumnConfig = {
  align?: ColumnAlign
  alignHeader?: ColumnAlign
  filter?: boolean
  header?: ReactNode
  hidden?: boolean
  style?: CSSProperties
}

function getColumnConfig(
  column: ColumnMeta,
  groupRowsBy?: string,
  hideControls?: boolean,
): ColumnConfig {
  const tooltipClassName = () => 'menuitemds-' + uuidv4()

  const getAlign = (
    align: ColumnAlign | null | undefined,
    fieldType: ColumnFieldType,
  ): ColumnAlign => {
    if (fieldType === 'image') {
      return 'center'
    }

    if (fieldType === 'isHidden') {
      return 'center'
    }

    if (align === undefined || align === null) {
      return 'left'
    }

    return align
  }

  const getAlignHeader = (
    align: ColumnAlign | undefined,
    fieldType: ColumnFieldType,
  ): ColumnAlign => {
    if (fieldType === 'image') {
      return 'center'
    }

    if (fieldType === 'isHidden') {
      return 'center'
    }

    if (!align) {
      return 'center'
    }

    return align
  }

  const getFilter = (
    filter: boolean | undefined,
    fieldType: ColumnFieldType,
  ): boolean | undefined => {
    if (fieldType === 'image') {
      return false
    }

    return filter
  }

  const getHeader = (
    field: string,
    header: string | undefined,
    fieldType: ColumnFieldType | undefined,
  ): ReactNode => {
    if (!fieldType) {
      return header ? header : camel2title(field)
    }

    switch (fieldType) {
      case 'blank':
        return <div />
      case 'epg':
        return 'EPG'
      case 'm3ulink':
        return 'M3U'
      case 'epglink':
        return 'XMLTV'
      case 'url':
        return 'HDHR'
      case 'streams':
        return (
          <>
            <Tooltip target={'.' + tooltipClassName} />
            <div
              className={tooltipClassName + ' border-white'}
              data-pr-at="right+5 top"
              data-pr-hidedelay={100}
              data-pr-my="left center-2"
              data-pr-position="right"
              data-pr-showdelay={200}
              data-pr-tooltip="Active/Total Count"
            >
              Streams
              <br />
              (active/total)
            </div>
          </>
        )
      default:
        return header ? header : camel2title(field)
    }
  }

  const getStyle = (
    style: CSSProperties | undefined,
    fieldType: ColumnFieldType | undefined,
  ): CSSProperties | undefined => {
    if (fieldType === 'blank') {
      return {
        maxWidth: '1rem',
        width: '1rem',
      } as CSSProperties
    }

    if (
      fieldType === 'image' ||
      fieldType === 'm3ulink' ||
      fieldType === 'epglink' ||
      fieldType === 'url'
    ) {
      return {
        maxWidth: '5rem',
        width: '5rem',
      } as CSSProperties
    }

    return {
      ...style,
      flexGrow: 0,
      flexShrink: 1,
      // maxWidth: '10rem',
      overflow: 'hidden',
      textOverflow: 'ellipsis',
      whiteSpace: 'nowrap',
    } as CSSProperties
  }

  return {
    align: getAlign(column.align, column.fieldType),
    alignHeader: getAlignHeader(column.align, column.fieldType),
    filter: getFilter(column.filter, column.fieldType),
    header: getHeader(column.field, column.header, column.fieldType),
    hidden:
      column.isHidden === true ||
      (hideControls === true &&
        getHeader(column.field, column.header, column.fieldType) === 'Actions')
        ? true
        : undefined,
    style: getStyle(column.style, column.fieldType),
  }
}

export default getColumnConfig
