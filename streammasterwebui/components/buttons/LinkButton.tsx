import useCopyToClipboard from '@/lib/hooks/useCopyToClipboard'
import { useState } from 'react'

export const LinkButton = ({ link }: { readonly link: string }) => {
  const [, copyToClipboard] = useCopyToClipboard()
  const [copied, setCopied] = useState(false)

  return (
    <div style={{ position: 'relative' }}>
      <div className="flex justify-content-center align-items-center">
        <a
          href={link}
          onClick={(e) => {
            e.preventDefault() // Prevent default behavior (navigation)

            void copyToClipboard(link).then((ifCopied) => {
              setCopied(ifCopied)
              setTimeout(() => setCopied(false), 750)
            })
          }}
          rel="noopener noreferrer"
          target="_blank"
          title="Copy link to clipboard"
        >
          {/* Conditionally render the icon based on the copied state */}
          <i className={copied ? 'pi pi-copy' : 'pi pi-bookmark-fill'} />
        </a>
      </div>
    </div>
  )
}
