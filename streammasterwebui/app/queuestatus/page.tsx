'use client'

import dynamic from "next/dynamic";

const QueueStatus = dynamic(() => import("@/features/queueStatus/QueueStatus"), { ssr: false })

export default function QueueStatusLayout() {
    return (<QueueStatus />);
}