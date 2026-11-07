import { Injectable } from "@nestjs/common";
import { PrismaService } from "../prisma/prisma.service.js";


@Injectable()
export class ClientService {
    constructor(private prisma: PrismaService) {}
}