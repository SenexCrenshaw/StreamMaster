'use client'
import '@/lib/styles/MenuItemSM.css'
import Link from 'next/link'
import { usePathname } from 'next/navigation'
import { Tooltip } from 'primereact/tooltip'
import React from 'react'
import { MenuItem } from 'react-pro-sidebar'
import { v4 as uuidv4 } from 'uuid'

const MenuItemSM = (props: MenuItemSMProps) => {
  const tooltipClassName = React.useMemo(() => 'menuitemsm-' + uuidv4(), [])

  const pathname = usePathname()

  if (props.collapsed) {
    return (
      <>
        <Tooltip target={'.' + tooltipClassName} />
        <div
          className={tooltipClassName + ' border-white'}
          data-pr-hidedelay={100}
          data-pr-position="right"
          data-pr-showdelay={500}
          data-pr-tooltip={props.tooltip ?? props.name}
        >
          <MenuItem
            active={pathname === props.link}
            component={
              <Link
                className="link"
                target={
                  props.newWindow === null
                    ? ''
                    : props.newWindow
                    ? '_blank'
                    : ''
                }
                href={props.link}
              />
            }
            icon={props.icon}
          >
            {props.children}
            {props.name}
          </MenuItem>
        </div>
      </>
    )
  }

  return (
    <MenuItem
      active={pathname === props.link}
      component={
        <Link
          className="link"
          target={
            props.newWindow === null ? '' : props.newWindow ? '_blank' : ''
          }
          href={props.link}
        />
      }
      icon={props.icon}
    >
      {props.children}
      {props.name}
    </MenuItem>
  )
}

export type MenuItemSMProps = {
  readonly children?: React.ReactNode
  readonly collapsed?: boolean
  readonly icon: React.ReactNode
  readonly link: string
  readonly name: string
  readonly newWindow?: boolean
  readonly tooltip?: string
}

export default React.memo(MenuItemSM)
