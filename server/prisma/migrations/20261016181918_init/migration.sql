-- CreateEnum
CREATE TYPE "SessionStatus" AS ENUM ('PENDING', 'STARTED', 'RUNNING_PRE_PROCESSES', 'RUNNING_MAIN_PROCESS', 'RUNNING_POST_PROCESSES', 'COMPLETED', 'ABORTED', 'FAILED');

-- CreateEnum
CREATE TYPE "ProcessPhase" AS ENUM ('PRE', 'MAIN', 'POST');

-- CreateEnum
CREATE TYPE "ProcessType" AS ENUM ('EXECUTABLE', 'LUA_SCRIPT');

-- CreateEnum
CREATE TYPE "ProcessStatus" AS ENUM ('PENDING', 'RUNNING', 'COMPLETED', 'FAILED');

-- CreateTable
CREATE TABLE "Client" (
    "id" TEXT NOT NULL,
    "machineName" TEXT NOT NULL,
    "ipAddress" TEXT,
    "connected" BOOLEAN NOT NULL DEFAULT false,
    "lastSeen" TIMESTAMP(3) NOT NULL,
    "createdAt" TIMESTAMP(3) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "Client_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "Session" (
    "id" TEXT NOT NULL,
    "clientId" TEXT NOT NULL,
    "status" "SessionStatus" NOT NULL DEFAULT 'PENDING',
    "startedAt" TIMESTAMP(3) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "endedAt" TIMESTAMP(3),
    "createdAt" TIMESTAMP(3) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" TIMESTAMP(3) NOT NULL,

    CONSTRAINT "Session_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "Process" (
    "id" TEXT NOT NULL,
    "sessionId" TEXT NOT NULL,
    "phase" "ProcessPhase" NOT NULL,
    "type" "ProcessType" NOT NULL DEFAULT 'EXECUTABLE',
    "target" TEXT,
    "arguments" TEXT,
    "scriptContent" TEXT,
    "delayMs" INTEGER NOT NULL DEFAULT 0,
    "repeatCount" INTEGER NOT NULL DEFAULT 1,
    "order" INTEGER NOT NULL,
    "status" "ProcessStatus" NOT NULL DEFAULT 'PENDING',
    "exitCode" INTEGER,
    "createdAt" TIMESTAMP(3) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" TIMESTAMP(3) NOT NULL,

    CONSTRAINT "Process_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "SubProcessLog" (
    "id" TEXT NOT NULL,
    "processId" TEXT NOT NULL,
    "processName" TEXT NOT NULL,
    "winPid" INTEGER NOT NULL,

    CONSTRAINT "SubProcessLog_pkey" PRIMARY KEY ("id")
);

-- CreateIndex
CREATE UNIQUE INDEX "Client_machineName_key" ON "Client"("machineName");

-- CreateIndex
CREATE INDEX "Process_sessionId_order_idx" ON "Process"("sessionId", "order");

-- CreateIndex
CREATE INDEX "SubProcessLog_processId_idx" ON "SubProcessLog"("processId");

-- AddForeignKey
ALTER TABLE "Session" ADD CONSTRAINT "Session_clientId_fkey" FOREIGN KEY ("clientId") REFERENCES "Client"("id") ON DELETE CASCADE ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE "Process" ADD CONSTRAINT "Process_sessionId_fkey" FOREIGN KEY ("sessionId") REFERENCES "Session"("id") ON DELETE CASCADE ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE "SubProcessLog" ADD CONSTRAINT "SubProcessLog_processId_fkey" FOREIGN KEY ("processId") REFERENCES "Process"("id") ON DELETE CASCADE ON UPDATE CASCADE;
