'use client'

import dynamic from "next/dynamic";


const StreamingStatus = dynamic(() => import("@/features/streamingStatus/StreamingStatus"), { ssr: false })

export default function StreamingStatusLayout() {
    return (<StreamingStatus />);
}